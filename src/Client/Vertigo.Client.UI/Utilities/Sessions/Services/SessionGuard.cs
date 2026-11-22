using System;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.Utilities.Sessions.Services;

public sealed class SessionGuard : ISessionGuard
{
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ISessionTransportCleaner _transport;
    private string? _invalidationMessage;

    public SessionGuard(PlayerSession session, INavigator navigation, ISessionTransportCleaner transport)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(transport);
        _session = session;
        _navigation = navigation;
        _transport = transport;
    }

    public bool HasActiveSession()
    {
        bool isActive = _session.GetActiveSession() is not null;
        if (!isActive)
        {
            Invalidate();
        }

        return isActive;
    }

    public bool IsCurrent(SessionSnapshot session)
    {
        return ReferenceEquals(_session.GetActiveSession(), session);
    }

    public void Invalidate(SessionSnapshot? session = null, string? messageKey = null)
    {

        SessionSnapshot? current = session is null ? null : _session.GetActiveSession();
        if (current is not null && !ReferenceEquals(current, session))
        {
            return;
        }

        if (session is not null)
        {
            _session.Clear(session);
        }

        if (_session.GetActiveSession() is null)
        {
            if (messageKey is not null)
            {
                _invalidationMessage = messageKey;
            }

            _transport.DisconnectAll();
            _navigation.NavigateTo(NavigationPage.Access);
        }
    }

    public string? TakeInvalidationMessage()
    {
        string? message = _invalidationMessage;
        _invalidationMessage = null;
        return message;
    }
}
