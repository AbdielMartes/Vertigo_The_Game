using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Vertigo.Client.UI.ViewModels.Presentation;
using System.ServiceModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class LobbyViewModel : ObservableObject, IDisposable, INavigationAware
{
    private readonly ILobbyClient _lobbyClient;
    private readonly IChatClient _chatClient;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _lobbyCode = string.Empty;
    private string _messageText = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private Guid _hostPlayerId;
    private bool _isMatchStarted;
    private bool _disposed;
    private long _lobbyGeneration;
    private readonly LobbyPreparationRevision _preparationRevision = new();

    public LobbyViewModel(ILobbyClient lobbyClient, IChatClient chatClient, LobbyContext context)
    {
        _lobbyClient = lobbyClient;
        _chatClient = chatClient;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        Match = context.Match;
        Members.CollectionChanged += (_, _) => RefreshMemberRows();
        _lobbyClient.PlayerJoined += OnPlayerJoined;
        _lobbyClient.PlayerLeft += OnPlayerLeft;
        _lobbyClient.MessageReceived += OnMessageReceived;
        _lobbyClient.PreparationChanged += OnPreparationChanged;
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
        LeaveCommand = new AsyncRelayCommand(LeaveAsync);
        ToggleReadyCommand = new AsyncRelayCommand(ToggleReadyAsync);
        StartMatchCommand = new AsyncRelayCommand(StartMatchAsync);
    }

    public MatchViewModel Match { get; }

    public ObservableCollection<LobbyMember> Members { get; } = new();
    public ObservableCollection<LobbyMemberRow> MemberRows { get; } = new();
    public ObservableCollection<LobbyChatMessage> Messages { get; } = new();
    public ObservableCollection<Guid> ReadyPlayerIds { get; } = new();

    public Guid HostPlayerId
    {
        get => _hostPlayerId;
        private set => SetProperty(ref _hostPlayerId, value);
    }

    public bool IsMatchStarted
    {
        get => _isMatchStarted;
        private set
        {
            if (SetProperty(ref _isMatchStarted, value))
            {
                OnPropertyChanged(nameof(MatchStatusText));
                OnPropertyChanged(nameof(CanPrepare));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public string HostUsername => Members.FirstOrDefault(m => m.PlayerId == HostPlayerId)?.Username ?? string.Empty;
    public bool IsHost => _playerSession.GetActiveSession()?.PlayerId == HostPlayerId;
    public bool IsReady => _playerSession.GetActiveSession() is { } active && ReadyPlayerIds.Contains(active.PlayerId);

    public string LobbyCode
    {
        get => _lobbyCode;
        private set => SetProperty(ref _lobbyCode, value);
    }

    public string MessageText
    {
        get => _messageText;
        set => SetProperty(ref _messageText, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(CanPrepare));
            }
        }
    }

    public bool CanPrepare => !IsBusy && !IsMatchStarted;

    public string RoomTitleText => _text.Get("Room.Title");
    public string CodeTitleText => _text.Get("Room.Code");
    public string LeaveText => _text.Get("Room.Leave");
    public string MembersTitleText => _text.Get("Room.Members");
    public string MembersDescriptionText => _text.Get("Room.MembersDescription");
    public string PreparationTitleText => _text.Get("Room.Preparation");
    public string PreparationHintText => _text.Get("Room.PreparationHint");
    public string HostLabelText => _text.Get("Room.Host");
    public string MatchStatusText => _text.Get(IsMatchStarted ? "Room.InGame" : "Room.Waiting");
    public string ReadyActionText => _text.Get(IsReady ? "Room.NotReadyAction" : "Room.ReadyAction");
    public string StartText => _text.Get("Room.Start");
    public string ChatTitleText => _text.Get("Room.Chat");
    public string MessagePlaceholderText => _text.Get("Room.MessagePlaceholder");
    public string SendText => _text.Get("Room.Send");
    public string ErrorHeadingText => _text.Get("Room.ErrorHeading");

    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand LeaveCommand { get; }
    public IAsyncRelayCommand ToggleReadyCommand { get; }
    public IAsyncRelayCommand StartMatchCommand { get; }

    public async Task<bool> CreateAsync(bool isPublic, int maxPlayers)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            string code = await Task.Run(() => _lobbyClient.CreateLobby(isPublic, maxPlayers, session.Token));
            if (!IsCurrentSession(session))
            {
                return false;
            }
            Members.Clear();
            Messages.Clear();
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = code;
            HostPlayerId = session.PlayerId;
            ReadyPlayerIds.Clear();
            _preparationRevision.Reset();
            IsMatchStarted = false;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            await RefreshPreparationAsync(session);
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.CreateFailed");
        }
        finally { IsBusy = false; }
    }

    public async Task<bool> JoinAsync(string code)
    {
        if (IsBusy || !string.IsNullOrEmpty(LobbyCode)) return false;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return false; }
        if (string.IsNullOrWhiteSpace(code)) return Fail("Lobby.CodeRequired");

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            LobbyAdmission admission = await Task.Run(() => _lobbyClient.JoinLobby(code.Trim(), session.Token));
            if (!IsCurrentSession(session))
            {
                return false;
            }
            Members.Clear();
            Messages.Clear();
            foreach (LobbyMember member in admission.ExistingMembers) Members.Add(member);
            Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                IsGuest = session.IsGuest });
            LobbyCode = admission.LobbyCode;
            HostPlayerId = admission.HostPlayerId;
            ReadyPlayerIds.Clear();
            _preparationRevision.Reset();
            IsMatchStarted = false;
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            await RefreshPreparationAsync(session);
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.JoinFailed");
        }
        finally { IsBusy = false; }
    }

    private async Task RefreshPreparationAsync(SessionSnapshot session)
    {
        string code = LobbyCode;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.GetPreparation(code, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            // Creation/join already succeeded; do not report a failed admission.
            if (_errors.IsUnauthorized(exception))
            {
                _coordinator.Invalidate(session);
            }
        }
    }

    private async Task ToggleReadyAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode) || IsMatchStarted)
        {
            return;
        }
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
            return;
        }
        IsBusy = true;
        string code = LobbyCode;
        bool ready = !IsReady;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.SetReady(code, ready, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.ReadyFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task StartMatchAsync()
    {
        if (IsBusy || !IsHost || IsMatchStarted || string.IsNullOrEmpty(LobbyCode))
        {
            return;
        }
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
            return;
        }
        IsBusy = true;
        string code = LobbyCode;
        try
        {
            LobbyPreparation preparation = await Task.Run(() => _lobbyClient.StartMatch(code, session.Token));
            if (IsCurrentSession(session) && string.Equals(LobbyCode, code, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.StartFailed");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPreparationChanged(object? sender, LobbyPreparation preparation)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        long eventGeneration = _lobbyGeneration;
        Dispatcher.UIThread.Post(() =>
        {
            if (!_disposed && eventSession is not null && IsCurrentSession(eventSession)
                && eventGeneration == _lobbyGeneration
                && string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                ApplyPreparation(preparation);
            }
        });
    }

    private void ApplyPreparation(LobbyPreparation preparation)
    {
        if (!_preparationRevision.TryAdvance(preparation, LobbyCode))
        {
            return;
        }
        HostPlayerId = preparation.HostPlayerId;
        IsMatchStarted = preparation.IsInGame;
        ReadyPlayerIds.Clear();
        foreach (Guid playerId in preparation.ReadyPlayerIds)
        {
            ReadyPlayerIds.Add(playerId);
        }
        RefreshMemberRows();
        OnPropertyChanged(nameof(IsHost));
        OnPropertyChanged(nameof(IsReady));
        OnPropertyChanged(nameof(ReadyActionText));
        if (MatchEntryRules.CanEnter(preparation, LobbyCode))
        {
            bool activated = Match.Activate(preparation.MatchId, LobbyCode);
            if (activated)
            {
                _navigation.NavigateTo(NavigationPage.Match);
            }
        }
    }

    private async Task SendMessageAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(MessageText) || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        string text = MessageText.Trim();
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var result = await Task.Run(() => _chatClient.SendMessage(LobbyCode, text, session.Token));
            if (_errors.IsUnauthorized(result)) { _coordinator.Invalidate(session); return; }
            if (!IsCurrentSession(session))
            {
                return;
            }
            if (result.IsSuccess && result.Value) MessageText = string.Empty;
            else ErrorMessage = _errors.FromResult(result, "Lobby.MessageFailed");
        }
        catch (Exception exception) { HandleServiceFailure(exception, session, "Lobby.MessageFailed"); }
        finally { IsBusy = false; }
    }

    private async Task LeaveAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null) { _coordinator.Invalidate(); return; }
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            bool left = await Task.Run(() => _lobbyClient.LeaveLobby(LobbyCode, session.Token));
            if (!IsCurrentSession(session))
            {
                return;
            }
            if (!left) { Fail("Lobby.LeaveFailed"); return; }
            _lobbyClient.Disconnect();
            ResetState();
            _navigation.NavigateTo(NavigationPage.Home);
        }
        catch (Exception exception)
        {
            HandleServiceFailure(exception, session, "Lobby.LeaveFailed");
        }
        finally { IsBusy = false; }
    }

    private void OnPlayerJoined(object? sender, LobbyMember member)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            if (Members.All(existing => existing.PlayerId != member.PlayerId)) Members.Add(member);
        });
    }

    private void OnPlayerLeft(object? sender, Guid playerId)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (_disposed || string.IsNullOrEmpty(LobbyCode)) return;
            LobbyMember? member = Members.FirstOrDefault(existing => existing.PlayerId == playerId);
            if (member is not null) Members.Remove(member);
        });
    }

    private void OnMessageReceived(object? sender, LobbyChatMessage message)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            if (!_disposed && string.Equals(LobbyCode, message.LobbyCode, StringComparison.OrdinalIgnoreCase))
                Messages.Add(message);
        });
    }

    public void OnNavigatedTo()
    {
        RefreshRoomTexts();
        if (!_coordinator.HasActiveSession()) return;
        ErrorMessage = string.Empty;
    }

    public void OnNavigatedFrom()
    {
        // Keep the lobby connection only when moving back from a transient view.
        if (_navigation.CurrentPage == NavigationPage.Access) ResetState();
    }

    public void ResetState()
    {
        _lobbyGeneration++;
        _preparationRevision.Reset();
        Match.Reset();
        LobbyCode = string.Empty;
        MessageText = string.Empty;
        ErrorMessage = string.Empty;
        Members.Clear();
        Messages.Clear();
        ReadyPlayerIds.Clear();
        HostPlayerId = Guid.Empty;
        IsMatchStarted = false;
        RefreshMemberRows();
        OnPropertyChanged(nameof(IsHost));
        OnPropertyChanged(nameof(IsReady));
    }

    private bool IsCurrentSession(SessionSnapshot previous)
    {
        SessionSnapshot? current = _playerSession.GetActiveSession();
        return ReferenceEquals(current, previous);
    }

    private bool HandleServiceFailure(Exception exception, SessionSnapshot session, string fallbackKey)
    {
        if (_errors.IsUnauthorized(exception) || !_coordinator.HasActiveSession())
        {
            _coordinator.Invalidate(session);
            return false;
        }

        ErrorMessage = _errors.FromException(exception, fallbackKey);
        return false;
    }

    private bool Fail(string key)
    {
        ErrorMessage = _text.Get(key);
        return false;
    }

    private void RefreshMemberRows()
    {
        MemberRows.Clear();
        foreach (LobbyMember member in Members)
        {
            bool host = member.PlayerId == HostPlayerId;
            bool ready = ReadyPlayerIds.Contains(member.PlayerId);
            MemberRows.Add(new LobbyMemberRow
            {
                Username = member.Username,
                RoleText = _text.Get(host ? "Room.Host" : member.IsGuest ? "Room.Guest" : "Room.Player"),
                ReadyText = _text.Get(ready ? "Room.Ready" : "Room.NotReady"),
                IsReady = ready
            });
        }
        OnPropertyChanged(nameof(HostUsername));
    }

    private void RefreshRoomTexts()
    {
        OnPropertyChanged(nameof(RoomTitleText));
        OnPropertyChanged(nameof(CodeTitleText));
        OnPropertyChanged(nameof(LeaveText));
        OnPropertyChanged(nameof(MembersTitleText));
        OnPropertyChanged(nameof(MembersDescriptionText));
        OnPropertyChanged(nameof(PreparationTitleText));
        OnPropertyChanged(nameof(PreparationHintText));
        OnPropertyChanged(nameof(HostLabelText));
        OnPropertyChanged(nameof(MatchStatusText));
        OnPropertyChanged(nameof(ReadyActionText));
        OnPropertyChanged(nameof(StartText));
        OnPropertyChanged(nameof(ChatTitleText));
        OnPropertyChanged(nameof(MessagePlaceholderText));
        OnPropertyChanged(nameof(SendText));
        OnPropertyChanged(nameof(ErrorHeadingText));
        RefreshMemberRows();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lobbyClient.PlayerJoined -= OnPlayerJoined;
        _lobbyClient.PlayerLeft -= OnPlayerLeft;
        _lobbyClient.MessageReceived -= OnMessageReceived;
        _lobbyClient.PreparationChanged -= OnPreparationChanged;
    }
}
