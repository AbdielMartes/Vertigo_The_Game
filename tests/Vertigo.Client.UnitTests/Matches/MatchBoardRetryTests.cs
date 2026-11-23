using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchBoardRetryTests
{
    [Fact]
    public void Schedule_IncreasesDelayUntilLimitThenResetsAfterSuccess()
    {
        TestClock clock = new();
        MatchBoardRetry retry = new(clock);
        foreach (int seconds in new[] { 5, 10, 20, 30, 30 })
        {
            retry.Schedule();
            clock.Advance(seconds - 1);
            Assert.True(retry.IsPending);
            Assert.False(retry.IsDue);
            clock.Advance(1);
            Assert.True(retry.IsDue);
            retry.BeginAttempt();
            Assert.False(retry.IsPending);
            Assert.False(retry.IsDue);
        }

        retry.Reset();
        retry.Schedule();
        clock.Advance(5);
        Assert.True(retry.IsDue);
    }

    private sealed class TestClock : TimeProvider
    {
        private long _timestamp;

        public override long TimestampFrequency => 1;

        public override long GetTimestamp() => _timestamp;

        public void Advance(int seconds)
        {
            _timestamp += seconds;
        }
    }
}
