using System;

namespace Vertigo.Client.UI.ViewModels.Presentation;

internal sealed class MatchTurnClock
{
    private readonly TimeProvider _timeProvider;
    private TimeSpan? _remainingAtSample;
    private long _sampleTimestamp;

    public MatchTurnClock(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public long GetTimestamp()
    {
        return _timeProvider.GetTimestamp();
    }

    public void SetDeadline(DateTimeOffset? deadlineUtc, DateTimeOffset? serverTimeUtc, long? receivedAt = null)
    {
        _sampleTimestamp = serverTimeUtc.HasValue && receivedAt.HasValue ? receivedAt.Value : GetTimestamp();
        _remainingAtSample = deadlineUtc - (serverTimeUtc ?? _timeProvider.GetUtcNow());
    }

    public int? GetRemainingSeconds()
    {
        int? seconds = null;
        if (_remainingAtSample is TimeSpan remaining)
        {
            remaining -= _timeProvider.GetElapsedTime(_sampleTimestamp);
            seconds = (int)Math.Max(0, Math.Ceiling(remaining.TotalSeconds));
        }

        return seconds;
    }
}
