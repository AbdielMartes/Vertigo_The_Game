using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UnitTests.Sessions;

public sealed class PlayerSessionTests
{
    [Fact]
    public void SetSession_ValidSession_AuthenticatesPlayer()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot snapshot = CreateSnapshot("token-a");

        playerSession.SetSession(snapshot);

        Assert.True(playerSession.IsAuthenticated);
        Assert.Same(snapshot, playerSession.GetActiveSession());
    }

    [Fact]
    public void SetSession_ExpiredSession_RejectsInvalidData()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot expired = CreateSnapshot("token-a", DateTime.UtcNow.AddMinutes(-1));

        Assert.Throws<ArgumentException>(() => playerSession.SetSession(expired));
        Assert.False(playerSession.IsAuthenticated);
    }

    [Fact]
    public void Clear_OlderSnapshot_DoesNotCloseNewSession()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot oldSession = CreateSnapshot("token-a");
        SessionSnapshot newSession = CreateSnapshot("token-b");
        playerSession.SetSession(oldSession);
        playerSession.SetSession(newSession);

        playerSession.Clear(oldSession);

        Assert.Same(newSession, playerSession.GetActiveSession());
    }

    [Fact]
    public void Clear_CurrentSnapshot_EndsSession()
    {
        PlayerSession playerSession = new(TimeProvider.System);
        SessionSnapshot snapshot = CreateSnapshot("token-a");
        playerSession.SetSession(snapshot);

        playerSession.Clear(snapshot);

        Assert.False(playerSession.IsAuthenticated);
    }

    internal static SessionSnapshot CreateSnapshot(string token, DateTime? expiresAt = null)
    {
        return new SessionSnapshot
        {
            PlayerId = Guid.NewGuid(),
            Username = "test_player",
            Token = token,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddMinutes(30),
            IsGuest = false
        };
    }
}
