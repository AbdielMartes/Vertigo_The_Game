using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class SocialViewModel : ObservableObject, INavigationAware, IDisposable
{
    private readonly IFriendshipNotificationClient _notifications;
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ISessionGuard _guard;
    private readonly IClientErrorText _errors;
    private bool _disposed;
    private bool _active;
    private bool _subscribed;
    private bool _isRefreshing;
    private int _generation;
    private SessionSnapshot? _subscribedSession;
    private string _errorMessage = string.Empty;

    public SocialViewModel(FriendsViewModel friends, BlockedPlayersViewModel blocked,
        SocialViewModelContext context)
    {
        Friends = friends;
        Blocked = blocked;
        Invitations = context.Invitations;
        _notifications = context.Notifications;
        _session = context.NavigationContext.Session;
        _navigation = context.NavigationContext.Navigation;
        _guard = context.NavigationContext.Services.SessionGuard;
        _errors = context.NavigationContext.Services.Errors;
        BackCommand = new RelayCommand(() => _navigation.NavigateTo(NavigationPage.Home));
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        _notifications.FriendRequestReceived += OnFriendRequest;
        _notifications.FriendshipEstablished += OnFriendship;
        _notifications.LobbyInvitationReceived += OnInvitation;
        _notifications.LobbyInvitationWithdrawn += OnInvitationWithdrawn;
    }

    public FriendsViewModel Friends { get; }
    public BlockedPlayersViewModel Blocked { get; }
    public LobbyInvitationsViewModel Invitations { get; }
    public string ErrorMessage { get => _errorMessage; private set => SetProperty(ref _errorMessage, value); }
    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }

    public void OnNavigatedTo()
    {
        _active = true;
        _ = RefreshAsync(); // Every exception is handled inside RefreshAsync.
    }

    public void OnNavigatedFrom()
    {
        _active = false;
        _generation++;
        _isRefreshing = false;
        _subscribed = false;
        _subscribedSession = null;
        _notifications.Disconnect();
    }

    public async Task RefreshAsync()
    {
        if (_disposed || !_active || _isRefreshing)
        {
            return;
        }
        _isRefreshing = true;
        int generation = _generation;
        try
        {
            SessionSnapshot? session = _session.GetActiveSession();
            if (session is null)
            {
                _guard.Invalidate();
                return;
            }
            if (session.IsGuest)
            {
                _navigation.NavigateTo(NavigationPage.Home);
                return;
            }
            if (!_subscribed || !ReferenceEquals(session, _subscribedSession))
            {
                try
                {
                    bool accepted = await Task.Run(() => _notifications.Subscribe(session.Token));
                    if (!IsActive(session, generation))
                    {
                        return;
                    }
                    if (!accepted)
                    {
                        ErrorMessage = _errors.FromException(new InvalidOperationException(),
                            "Social.NotificationsFailed");
                        return;
                    }
                    _subscribed = true;
                    _subscribedSession = session;
                }
                catch (Exception exception)
                {
                    if (!IsActive(session, generation))
                    {
                        return;
                    }
                    if (_errors.IsUnauthorized(exception))
                    {
                        _guard.Invalidate(session);
                        return;
                    }
                    ErrorMessage = _errors.FromException(exception, "Social.NotificationsFailed");
                    return;
                }
            }
            if (!IsActive(session, generation))
            {
                return;
            }
            await Friends.RefreshAsync();
            if (!IsActive(session, generation))
            {
                return;
            }
            await Blocked.RefreshAsync();
            if (!IsActive(session, generation))
            {
                return;
            }
            await Invitations.RefreshAsync();
        }
        finally
        {
            if (generation == _generation)
            {
                _isRefreshing = false;
            }
        }
    }

    private bool IsActive(SessionSnapshot session, int generation)
    {
        return !_disposed && _active && generation == _generation
            && ReferenceEquals(_session.GetActiveSession(), session);
    }

    private void OnFriendRequest(object? sender, FriendshipInvitationEntry entry)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Friends.PendingRequests.All(item => item.SenderId != entry.SenderId)) Friends.PendingRequests.Add(entry);
        });
    }

    private void OnFriendship(object? sender, FriendEntry friend)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Friends.Friends.All(item => item.PlayerId != friend.PlayerId)) Friends.Friends.Add(friend);
        });
    }

    private void OnInvitation(object? sender, LobbyInvitationEntry invitation)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            if (Invitations.PendingInvitations.All(item => item.LobbyCode != invitation.LobbyCode))
                Invitations.PendingInvitations.Add(invitation);
        });
    }

    private void OnInvitationWithdrawn(object? sender, string code)
    {
        SessionSnapshot? eventSession = _subscribedSession;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (eventSession is null || !IsActive(eventSession, generation))
            {
                return;
            }
            LobbyInvitationEntry? item = Invitations.PendingInvitations.FirstOrDefault(
                invitation => invitation.LobbyCode == code);
            if (item is not null) Invitations.PendingInvitations.Remove(item);
        });
    }

    public void Reset()
    {
        _active = false;
        _generation++;
        _isRefreshing = false;
        _subscribed = false;
        _subscribedSession = null;
        _notifications.Disconnect();
        ErrorMessage = string.Empty;
        Friends.Reset(); Blocked.Reset(); Invitations.Reset();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _notifications.FriendRequestReceived -= OnFriendRequest;
        _notifications.FriendshipEstablished -= OnFriendship;
        _notifications.LobbyInvitationReceived -= OnInvitation;
        _notifications.LobbyInvitationWithdrawn -= OnInvitationWithdrawn;
    }
}
