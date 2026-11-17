using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
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
    private bool _disposed;

    public LobbyViewModel(ILobbyClient lobbyClient, IChatClient chatClient, LobbyContext context)
    {
        _lobbyClient = lobbyClient;
        _chatClient = chatClient;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        _lobbyClient.PlayerJoined += OnPlayerJoined;
        _lobbyClient.PlayerLeft += OnPlayerLeft;
        _lobbyClient.MessageReceived += OnMessageReceived;
        SendMessageCommand = new AsyncRelayCommand(SendMessageAsync);
        LeaveCommand = new AsyncRelayCommand(LeaveAsync);
    }

    public ObservableCollection<LobbyMember> Members { get; } = new();
    public ObservableCollection<LobbyChatMessage> Messages { get; } = new();

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
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public IAsyncRelayCommand SendMessageCommand { get; }
    public IAsyncRelayCommand LeaveCommand { get; }

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
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
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
            _navigation.NavigateTo(NavigationPage.Lobby);
            _lobbyClient.ReplayPendingEvents();
            return true;
        }
        catch (Exception exception)
        {
            return HandleServiceFailure(exception, session, "Lobby.JoinFailed");
        }
        finally { IsBusy = false; }
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
        LobbyCode = string.Empty;
        MessageText = string.Empty;
        ErrorMessage = string.Empty;
        Members.Clear();
        Messages.Clear();
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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lobbyClient.PlayerJoined -= OnPlayerJoined;
        _lobbyClient.PlayerLeft -= OnPlayerLeft;
        _lobbyClient.MessageReceived -= OnMessageReceived;
    }
}
