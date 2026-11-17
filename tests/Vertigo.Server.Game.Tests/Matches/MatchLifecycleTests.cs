using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchLifecycleTests
{
    [Fact]
    public void Start_ValidRequest_EntersRunningWithRevisionOne()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchSessionSnapshot session = starter.Start(CreateRequest());

        MatchLifecycleSnapshot? state = starter.Get(session.MatchId);

        Assert.NotNull(state);
        Assert.Equal(MatchLifecycleStatus.Running, state.Status);
        Assert.Equal(1, state.Revision);
        Assert.Equal(session.LobbyInstanceId, state.LobbyInstanceId);
    }

    [Fact]
    public void TryFinish_RunningMatch_ChangesToFinished()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;

        bool finished = starter.TryFinish(matchId);

        Assert.True(finished);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void TryCancel_RunningMatch_ChangesToCancelled()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;

        bool cancelled = starter.TryCancel(matchId);

        Assert.True(cancelled);
        Assert.Equal(MatchLifecycleStatus.Cancelled, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void TryFinish_FinishedMatch_LeavesRevisionUnchanged()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);

        bool repeated = starter.TryFinish(matchId);

        Assert.False(repeated);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void TryCancel_FinishedMatch_DoesNotChangeTerminalStatus()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);

        bool cancelled = starter.TryCancel(matchId);

        Assert.False(cancelled);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void TryFinish_UnknownMatch_ReturnsFalse()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);

        bool result = starter.TryFinish(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public void TryFinish_ConcurrentCalls_OnlyOneSucceeds()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        int succeeded = 0;

        Parallel.For(0, 64, _ =>
        {
            if (starter.TryFinish(matchId))
            {
                Interlocked.Increment(ref succeeded);
            }
        });

        Assert.Equal(1, succeeded);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void PurgeTerminated_RunningMatch_KeepsSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        clock.Advance(TimeSpan.FromDays(30));

        int removed = starter.PurgeTerminated(TimeSpan.Zero);

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_BeforeRetention_KeepsFinishedSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        starter.TryFinish(matchId);
        clock.Advance(TimeSpan.FromMinutes(9));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_AtRetention_RemovesFinishedSession()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        MatchStartRequest request = CreateRequest();
        Guid matchId = starter.Start(request).MatchId;
        starter.TryFinish(matchId);
        clock.Advance(TimeSpan.FromMinutes(10));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_NegativeRetention_ThrowsArgumentOutOfRangeException()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);

        Assert.Throws<ArgumentOutOfRangeException>(() => starter.PurgeTerminated(TimeSpan.FromSeconds(-1)));
    }

    [Fact]
    public void Start_RepeatedAfterFinish_ReturnsOriginalMatch()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        MatchSessionSnapshot first = starter.Start(request);
        starter.TryFinish(first.MatchId);

        MatchSessionSnapshot second = starter.Start(request);

        Assert.Same(first, second);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(second.MatchId)?.Status);
    }

    [Fact]
    public void TryCancel_ConcurrentWithFinish_ChoosesOneTerminalState()
    {
        InMemoryMatchSessionStarter starter = new(TimeProvider.System);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        bool[] results = new bool[2];

        Parallel.Invoke(
            () => results[0] = starter.TryFinish(matchId),
            () => results[1] = starter.TryCancel(matchId));

        Assert.Single(results, result => result);
        Assert.Equal(2, starter.Get(matchId)?.Revision);
    }

    [Fact]
    public void PurgeTerminated_MixedSessions_RemovesOnlyEligibleAndKeepsLobbyIndexConsistent()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        MatchStartRequest eligibleRequest = CreateRequest();
        MatchStartRequest recentRequest = CreateRequest();
        MatchStartRequest runningRequest = CreateRequest();
        MatchSessionSnapshot eligible = starter.Start(eligibleRequest);
        Assert.True(starter.TryFinish(eligible.MatchId));

        clock.Advance(TimeSpan.FromMinutes(7));
        MatchSessionSnapshot recent = starter.Start(recentRequest);
        Assert.True(starter.TryCancel(recent.MatchId));
        MatchSessionSnapshot running = starter.Start(runningRequest);
        clock.Advance(TimeSpan.FromMinutes(3));

        int removed = starter.PurgeTerminated(TimeSpan.FromMinutes(10));

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(eligible.MatchId));
        Assert.Equal(MatchLifecycleStatus.Cancelled, starter.Get(recent.MatchId)?.Status);
        Assert.Equal(MatchLifecycleStatus.Running, starter.Get(running.MatchId)?.Status);
        Assert.Same(recent, starter.Start(recentRequest));
        Assert.Same(running, starter.Start(runningRequest));
        MatchSessionSnapshot recreated = starter.Start(eligibleRequest);
        Assert.NotEqual(eligible.MatchId, recreated.MatchId);
        Assert.Equal(MatchLifecycleStatus.Running, starter.Get(recreated.MatchId)?.Status);
    }

    [Fact]
    public void PurgeTerminated_ExtremePositiveRetention_DoesNotOverflowOrPurge()
    {
        FakeClock clock = new();
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.MaxValue);

        Assert.Equal(0, removed);
        Assert.NotNull(starter.Get(matchId));
    }

    [Fact]
    public void PurgeTerminated_RetentionBeyondRepresentablePast_KeepsTerminalAtMinimumDate()
    {
        FakeClock clock = new();
        clock.Set(DateTimeOffset.MinValue);
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.FromTicks(1));

        Assert.Equal(0, removed);
        Assert.Equal(MatchLifecycleStatus.Finished, starter.Get(matchId)?.Status);
    }

    [Fact]
    public void PurgeTerminated_AtMinimumDateWithZeroRetention_RemovesTerminal()
    {
        FakeClock clock = new();
        clock.Set(DateTimeOffset.MinValue);
        InMemoryMatchSessionStarter starter = new(clock);
        Guid matchId = starter.Start(CreateRequest()).MatchId;
        Assert.True(starter.TryFinish(matchId));

        int removed = starter.PurgeTerminated(TimeSpan.Zero);

        Assert.Equal(1, removed);
        Assert.Null(starter.Get(matchId));
    }

    private static MatchStartRequest CreateRequest()
    {
        Guid host = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };

        return request;
    }

    private sealed class FakeClock : TimeProvider
    {
        private DateTimeOffset _now = new(2026, 11, 16, 0, 0, 0, TimeSpan.Zero);

        public override DateTimeOffset GetUtcNow()
        {
            return _now;
        }

        public void Set(DateTimeOffset now)
        {
            _now = now;
        }

        public void Advance(TimeSpan duration)
        {
            _now = _now.Add(duration);
        }
    }
}
