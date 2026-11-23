using System;

namespace Vertigo.Client.UI.ViewModels.Lifecycle;

internal sealed class MatchBoardRetry(TimeProvider clock)
{
    private const int InitialDelaySeconds = 5;
    private const int MaximumDelaySeconds = 30;

    private int _delaySeconds;
    private long? _scheduledAt;

    public bool IsPending => _scheduledAt.HasValue;

    public void Schedule()
    {
        _delaySeconds = _delaySeconds == 0
            ? InitialDelaySeconds : Math.Min(MaximumDelaySeconds, _delaySeconds * 2);
        _scheduledAt = clock.GetTimestamp();
    }

    public bool IsDue => _scheduledAt is long started
        && clock.GetElapsedTime(started) >= TimeSpan.FromSeconds(_delaySeconds);

    public void BeginAttempt()
    {
        _scheduledAt = null;
    }

    public void Reset()
    {
        BeginAttempt();
        _delaySeconds = 0;
    }
}
