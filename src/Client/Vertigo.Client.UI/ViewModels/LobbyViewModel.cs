using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Vertigo.Client.UI.Resources;
using Vertigo.Client.UI.ViewModels.Presentation;
using System.ServiceModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class LobbyViewModel : ObservableObject, IDisposable, INavigationAware
{
    private const int MaximumRetainedChatMessages = 100;

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
        ArgumentNullException.ThrowIfNull(lobbyClient);
        ArgumentNullException.ThrowIfNull(chatClient);
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.PlayerSession);
        ArgumentNullException.ThrowIfNull(context.Ui);
        ArgumentNullException.ThrowIfNull(context.Ui.Navigation);
        ArgumentNullException.ThrowIfNull(context.Ui.Services);
        ArgumentNullException.ThrowIfNull(context.Ui.Services.Errors);
        ArgumentNullException.ThrowIfNull(context.Ui.Services.SessionGuard);
        ArgumentNullException.ThrowIfNull(context.Ui.Services.Text);
        ArgumentNullException.ThrowIfNull(context.Match);
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
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync, CanSendMessage);
        LeaveCommand = new AsyncRelayCommand(LeaveAsync);
        ToggleReadyCommand = new AsyncRelayCommand(ToggleReadyAsync);
        StartMatchCommand = new AsyncRelayCommand(StartMatchAsync);
    }

    public MatchViewModel Match { get; }

    public ObservableCollection<LobbyMember> Members { get; } = new();
    public ObservableCollection<LobbyMemberRow> MemberRows { get; } = new();
    public ObservableCollection<MatchChatRow> Messages { get; } = new();
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
        private set
        {
            if (SetProperty(ref _lobbyCode, value))
            {
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string MessageText
    {
        get => _messageText;
        set
        {
            if (SetProperty(ref _messageText, value))
            {
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
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
                SendMessageCommand.NotifyCanExecuteChanged();
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
        bool succeeded = false;
        if (!IsBusy && string.IsNullOrEmpty(LobbyCode))
        {
            SessionSnapshot? session = _playerSession.GetActiveSession();
            if (session is null)
            {
                _coordinator.Invalidate();
            }
            else
            {
                succeeded = await CreateForSessionAsync(session, isPublic, maxPlayers);
            }
        }

        return succeeded;
    }

    private async Task<bool> CreateForSessionAsync(SessionSnapshot session, bool isPublic, int maxPlayers)
    {
        bool succeeded = false;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            string code = await Task.Run(() => _lobbyClient.CreateLobby(isPublic, maxPlayers, session.Token));
            if (IsCurrentSession(session))
            {
                Members.Clear();
                Messages.Clear();
                Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                    IsGuest = session.IsGuest, AvatarId = session.AvatarId });
                LobbyCode = code;
                HostPlayerId = session.PlayerId;
                ReadyPlayerIds.Clear();
                _preparationRevision.Reset();
                IsMatchStarted = false;
                _navigation.NavigateTo(NavigationPage.Lobby);
                _lobbyClient.ReplayPendingEvents();
                await RefreshPreparationAsync(session);
                succeeded = IsCurrentSession(session);
            }
        }
        catch (Exception ex)
        {
            HandleServiceFailure(ex, session, "Lobby.CreateFailed");
        }
        finally
        {
            IsBusy = false;
        }

        return succeeded;
    }

    public async Task<bool> JoinAsync(string code)
    {
        bool succeeded = false;
        if (!IsBusy && string.IsNullOrEmpty(LobbyCode))
        {
            SessionSnapshot? session = _playerSession.GetActiveSession();
            if (session is null)
            {
                _coordinator.Invalidate();
            }
            else if (string.IsNullOrWhiteSpace(code))
            {
                Fail("Lobby.CodeRequired");
            }
            else
            {
                succeeded = await JoinForSessionAsync(session, code);
            }
        }

        return succeeded;
    }

    private async Task<bool> JoinForSessionAsync(SessionSnapshot session, string code)
    {
        bool succeeded = false;
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            LobbyAdmission admission = await Task.Run(() => _lobbyClient.JoinLobby(code.Trim(), session.Token));
            if (IsCurrentSession(session))
            {
                Members.Clear();
                Messages.Clear();
                foreach (LobbyMember member in admission.ExistingMembers) Members.Add(member);
                Members.Add(new LobbyMember { PlayerId = session.PlayerId, Username = session.Username,
                    IsGuest = session.IsGuest, AvatarId = session.AvatarId });
                LobbyCode = admission.LobbyCode;
                HostPlayerId = admission.HostPlayerId;
                ReadyPlayerIds.Clear();
                _preparationRevision.Reset();
                IsMatchStarted = false;
                _navigation.NavigateTo(NavigationPage.Lobby);
                _lobbyClient.ReplayPendingEvents();
                await RefreshPreparationAsync(session);
                succeeded = IsCurrentSession(session);
            }
        }
        catch (Exception ex)
        {
            HandleServiceFailure(ex, session, "Lobby.JoinFailed");
        }
        finally
        {
            IsBusy = false;
        }

        return succeeded;
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
        catch (Exception ex) when (ex is CommunicationException or TimeoutException)
        {
            HandleServiceFailure(ex, session, "Error.Unknown");
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
        }
        else
        {
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
            catch (Exception ex)
            {
                HandleServiceFailure(ex, session, "Lobby.ReadyFailed");
            }
            finally
            {
                IsBusy = false;
            }
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
        }
        else
        {
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
            catch (Exception ex)
            {
                HandleServiceFailure(ex, session, "Lobby.StartFailed");
            }
            finally
            {
                IsBusy = false;
            }
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

    private bool CanSendMessage()
    {
        return !IsBusy && !string.IsNullOrEmpty(LobbyCode) && MatchChatRules.CanSend(MessageText);
    }

    private async Task SendMessageAsync()
    {
        if (!CanSendMessage())
        {
            return;
        }

        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
        }
        else
        {
            string text = MessageText;
            string lobbyCode = LobbyCode;
            long generation = _lobbyGeneration;
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var result = await Task.Run(() => _chatClient.SendMessage(lobbyCode, text, session.Token));
                if (_errors.IsUnauthorized(result))
                {
                    _coordinator.Invalidate(session);
                }
                else if (IsCurrentSession(session) && generation == _lobbyGeneration)
                {
                    bool blocked = result.ErrorCode == ServiceErrorCodes.ChatLanguageBlocked;
                    if (blocked)
                    {
                        AddChatRow(MatchChatRow.Blocked(session.Username, _text.Get("Chat.ExplicitContent")));
                    }

                    if (blocked || (result.IsSuccess && result.Value))
                    {
                        if (MessageText == text)
                        {
                            MessageText = string.Empty;
                        }
                    }
                    else
                    {
                        ErrorMessage = _errors.FromResult(result, "Lobby.MessageFailed");
                    }
                }
            }
            catch (Exception ex)
            {
                if (generation == _lobbyGeneration)
                {
                    HandleServiceFailure(ex, session, "Lobby.MessageFailed");
                }
            }
            finally { IsBusy = false; }
        }
    }

    private async Task LeaveAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(LobbyCode)) return;
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
        }
        else
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                bool left = await Task.Run(() => _lobbyClient.LeaveLobby(LobbyCode, session.Token));
                if (IsCurrentSession(session))
                {
                    if (left)
                    {
                        _lobbyClient.Disconnect();
                        ResetState();
                        _navigation.NavigateTo(NavigationPage.Home);
                    }
                    else
                    {
                        Fail("Lobby.LeaveFailed");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleServiceFailure(ex, session, "Lobby.LeaveFailed");
            }
            finally { IsBusy = false; }
        }
    }

    private void OnPlayerJoined(object? sender, LobbyMember member)
    {
        SessionSnapshot? eventSession = _playerSession.GetActiveSession();
        string eventLobby = LobbyCode;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !ReferenceEquals(_playerSession.GetActiveSession(), eventSession)
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase)
                || _disposed || string.IsNullOrEmpty(LobbyCode))
            {
                return;
            }
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
                || !string.Equals(LobbyCode, eventLobby, StringComparison.OrdinalIgnoreCase)
                || _disposed || string.IsNullOrEmpty(LobbyCode))
            {
                return;
            }
            LobbyMember? member = Members.FirstOrDefault(existing => existing.PlayerId == playerId);
            if (member is not null) Members.Remove(member);
        });
    }

    private void OnMessageReceived(object? sender, LobbyChatMessage message)
    {
        if (message.MatchId.HasValue)
        {
            return;
        }

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
            {
                AddChatRow(MatchChatRow.FromMessage(message));
            }
        });
    }

    private void AddChatRow(MatchChatRow row)
    {
        Messages.Add(row);
        if (Messages.Count > MaximumRetainedChatMessages)
        {
            Messages.RemoveAt(0);
        }
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

    private void HandleServiceFailure(Exception ex, SessionSnapshot session, string fallbackKey)
    {
        if (ex is not FaultException<ServiceFault>)
        {
            Debug.WriteLine($"Lobby action {fallbackKey} failed: {ex.GetType().Name}");
        }

        if (_errors.IsUnauthorized(ex))
        {
            _coordinator.Invalidate(session, ServiceMessageKeys.Unauthorized);
        }
        else if (_coordinator.IsCurrent(session))
        {
            ErrorMessage = _errors.FromException(ex, fallbackKey);
        }
        else if (_playerSession.GetActiveSession() is null)
        {
            _coordinator.Invalidate(session);
        }
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
                Avatar = AvatarCatalog.Resolve(member.AvatarId),
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
