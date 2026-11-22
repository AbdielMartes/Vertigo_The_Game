using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;
using Vertigo.Contracts.Faults;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class SessionGuardTests
{
    [Fact]
    public void Invalidate_CurrentSession_ClosesChannelsAndNavigatesToAccess()
    {
        PlayerSession sessions = new(TimeProvider.System);
        SessionSnapshot current = PlayerSessionTests.CreateSnapshot("token-1");
        sessions.SetSession(current);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        guard.Invalidate(current, ServiceMessageKeys.Unauthorized);

        Assert.False(sessions.IsAuthenticated);
        Assert.Equal(NavigationPage.Access, navigator.CurrentPage);
        Assert.Equal(1, transport.DisconnectCount);
        Assert.Equal(ServiceMessageKeys.Unauthorized, guard.TakeInvalidationMessage());
        Assert.Null(guard.TakeInvalidationMessage());
    }

    [Fact]
    public void Invalidate_StaleResponse_DoesNotCloseNewSessionOrTransport()
    {
        PlayerSession sessions = new(TimeProvider.System);
        SessionSnapshot previous = PlayerSessionTests.CreateSnapshot("token-1");
        SessionSnapshot current = PlayerSessionTests.CreateSnapshot("token-2");
        sessions.SetSession(previous);
        sessions.SetSession(current);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        guard.Invalidate(previous, ServiceMessageKeys.Unauthorized);

        Assert.Same(current, sessions.GetActiveSession());
        Assert.Equal(NavigationPage.Home, navigator.CurrentPage);
        Assert.Equal(0, transport.DisconnectCount);
        Assert.Null(guard.TakeInvalidationMessage());
    }

    [Fact]
    public void HasActiveSession_WithoutSession_ReturnsFalseAndNavigatesToAccess()
    {
        PlayerSession sessions = new(TimeProvider.System);
        Navigator navigator = new();
        navigator.NavigateTo(NavigationPage.Home);
        FakeTransportCleaner transport = new();
        SessionGuard guard = new(sessions, navigator, transport);

        bool active = guard.HasActiveSession();

        Assert.False(active);
        Assert.Equal(NavigationPage.Access, navigator.CurrentPage);
        Assert.Equal(1, transport.DisconnectCount);
    }

    private sealed class FakeTransportCleaner : ISessionTransportCleaner
    {
        public int DisconnectCount { get; private set; }

        public void DisconnectAll()
        {
            DisconnectCount++;
        }
    }
}
