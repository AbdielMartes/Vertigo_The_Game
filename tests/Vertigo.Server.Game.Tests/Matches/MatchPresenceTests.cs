using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchPresenceTests
{
    [Fact]
    public void TryDisconnect_OnePlayer_MatchRemainsRunning()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.True(registry.TryDisconnect(match.MatchId, first));
        Assert.Equal(MatchLifecycleStatus.Running, registry.Get(match.MatchId)?.Status);
        Assert.Equal(new[] { first }, registry.GetPresence(match.MatchId)?.DisconnectedPlayerIds);
    }

    [Fact]
    public void TryDisconnect_AllPlayers_SuspendsMatch()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        Assert.Equal(MatchLifecycleStatus.Suspended, registry.Get(match.MatchId)?.Status);
        Assert.Equal(3, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryReconnect_SuspendedMatch_RestoresRunning()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        MatchReconnectResult restored = registry.TryReconnect(match.MatchId, first);

        Assert.Equal(MatchReconnectResult.Reconnected, restored);
        Assert.Equal(MatchLifecycleStatus.Running, registry.Get(match.MatchId)?.Status);
        Assert.Equal(match.MatchId, registry.GetPresence(match.MatchId)?.MatchId);
    }

    [Fact]
    public void TryReconnect_DuplicateCall_PreservesRevision()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryReconnect(match.MatchId, first);
        long revision = registry.Get(match.MatchId)!.Revision;

        Assert.Equal(MatchReconnectResult.AlreadyConnected, registry.TryReconnect(match.MatchId, first));
        Assert.Equal(revision, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryDisconnect_UnknownParticipant_IsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.False(registry.TryDisconnect(match.MatchId, Guid.NewGuid()));
        Assert.Equal(1, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryReconnect_FinishedMatch_IsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryFinish(match.MatchId);
        long revision = registry.Get(match.MatchId)!.Revision;

        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, first));
        Assert.Equal(revision, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public void TryFinish_SuspendedMatch_IsAllowed()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        registry.TryDisconnect(match.MatchId, second);

        Assert.True(registry.TryFinish(match.MatchId));
        Assert.Equal(MatchLifecycleStatus.Finished, registry.Get(match.MatchId)?.Status);
    }

    [Fact]
    public void TryReconnect_UnknownMatchOrPlayer_ReturnsRejected()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);

        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(Guid.NewGuid(), first));
        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, Guid.NewGuid()));
        Assert.Equal(1, registry.Get(match.MatchId)?.Revision);
    }

    [Fact]
    public async Task TryReconnect_ConcurrentFinish_DoesNotReviveTerminalMatch()
    {
        InMemoryMatchSessionStarter registry = new(TimeProvider.System);
        (MatchSessionSnapshot match, Guid first, Guid second) = Start(registry);
        registry.TryDisconnect(match.MatchId, first);
        using Barrier barrier = new(2);

        Task<MatchReconnectResult> reconnect = Task.Run(() =>
        {
            barrier.SignalAndWait();
            return registry.TryReconnect(match.MatchId, first);
        });
        Task<bool> finish = Task.Run(() =>
        {
            barrier.SignalAndWait();
            return registry.TryFinish(match.MatchId);
        });
        await Task.WhenAll(reconnect, finish);

        Assert.True(await finish);
        Assert.Equal(MatchLifecycleStatus.Finished, registry.Get(match.MatchId)?.Status);
        Assert.Contains(await reconnect, new[] { MatchReconnectResult.Reconnected, MatchReconnectResult.Rejected });
        Assert.Equal(MatchReconnectResult.Rejected, registry.TryReconnect(match.MatchId, first));
    }

    private static (MatchSessionSnapshot Match, Guid First, Guid Second) Start(InMemoryMatchSessionStarter registry)
    {
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC123",
            HostPlayerId = first,
            PlayerIds = new[] { first, second }
        };
        return (registry.Start(request), first, second);
    }
}
