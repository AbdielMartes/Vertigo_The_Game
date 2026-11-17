using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class SessionCoordinatorTests
{
    [Fact]
    public void Login_Failed_LeavesPreviousSessionUntouched()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot previous = PlayerSessionTests.CreateSnapshot("old");
        state.SetSession(previous);
        FakeAuthClient client = new() { LoginResult = ClientResult<UserSession>.BusinessFailure("INVALID_CREDENTIALS", "ErrorInvalidCredentials") };
        SessionCoordinator coordinator = new(client, state);

        ClientResult<UserSession> result = coordinator.Login("wrong", "wrong");

        Assert.False(result.IsSuccess);
        Assert.Same(previous, state.GetActiveSession());
    }

    [Fact]
    public void Logout_Failed_KeepsSession()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot active = PlayerSessionTests.CreateSnapshot("current");
        state.SetSession(active);
        FakeAuthClient client = new() { LogoutResult = ClientResult<bool>.TechnicalFailure(FailureKind.ServerUnreachable) };
        SessionCoordinator coordinator = new(client, state);

        ClientResult<bool> result = coordinator.Logout();

        Assert.False(result.IsSuccess);
        Assert.Same(active, state.GetActiveSession());
    }

    [Fact]
    public async Task Logout_LateSuccess_DoesNotClearReplacementSession()
    {
        PlayerSession state = new(TimeProvider.System);
        SessionSnapshot oldSession = PlayerSessionTests.CreateSnapshot("old");
        SessionSnapshot newSession = PlayerSessionTests.CreateSnapshot("new");
        state.SetSession(oldSession);
        using ManualResetEventSlim started = new(false);
        using ManualResetEventSlim release = new(false);
        FakeAuthClient client = new()
        {
            OnLogout = _ =>
            {
                started.Set();
                if (!release.Wait(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("Test synchronization timed out.");
                }
                return ClientResult<bool>.Success(true);
            }
        };
        SessionCoordinator coordinator = new(client, state);

        Task<ClientResult<bool>> pending = Task.Run(coordinator.Logout);
        Assert.True(started.Wait(TimeSpan.FromSeconds(5)));
        try
        {
            state.SetSession(newSession);
        }
        finally
        {
            release.Set();
        }
        ClientResult<bool> result = await pending;

        Assert.True(result.IsSuccess);
        Assert.Same(newSession, state.GetActiveSession());
    }

    private sealed class FakeAuthClient : IAuthClient
    {
        public ClientResult<UserSession> LoginResult { get; set; } = ClientResult<UserSession>.TechnicalFailure(FailureKind.Unexpected);
        public ClientResult<bool> LogoutResult { get; set; } = ClientResult<bool>.Success(true);
        public Func<string, ClientResult<bool>>? OnLogout { get; set; }

        public ClientResult<UserSession> Login(LoginCredentials request)
        {
            return LoginResult;
        }

        public ClientResult<UserSession> ResumeSession(string sessionToken)
        {
            return LoginResult;
        }

        public ClientResult<UserSession> CreateGuestSession()
        {
            return LoginResult;
        }

        public ClientResult<bool> Logout(string sessionToken)
        {
            return OnLogout?.Invoke(sessionToken) ?? LogoutResult;
        }
    }
}
