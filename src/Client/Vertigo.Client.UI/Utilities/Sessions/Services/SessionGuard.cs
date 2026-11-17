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

    public SessionGuard(PlayerSession session, INavigator navigation, ISessionTransportCleaner transport)
    {
        _session = session;
        _navigation = navigation;
        _transport = transport;
    }

    public bool HasActiveSession()
    {
        if (_session.GetActiveSession() is not null)
        {
            return true;
        }

        Invalidate();
        return false;
    }

    public bool IsCurrent(SessionSnapshot session)
    {
        return ReferenceEquals(_session.GetActiveSession(), session);
    }

    public void Invalidate(SessionSnapshot? session = null)
    {
        // A late response from an older session cannot close the current channels.
        if (session is not null)
        {
            SessionSnapshot? current = _session.GetActiveSession();
            if (current is not null && !ReferenceEquals(current, session))
            {
                return;
            }
            _session.Clear(session);
        }

        if (_session.GetActiveSession() is not null)
        {
            return;
        }

        _transport.DisconnectAll();
        _navigation.NavigateTo(NavigationPage.Access);
    }
}
