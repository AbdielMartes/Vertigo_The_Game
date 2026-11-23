using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Host.Transport;
using Vertigo.Server.PlayerManagement.Statistics;

namespace Vertigo.Server.Host.Matches;

internal sealed class MatchStatisticsWorker(
    ICompletedMatchSource matches, IServiceScopeFactory scopeFactory, ILogger<MatchStatisticsWorker> logger,
    TimeProvider clock)
    : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(5);
    private static readonly EventId FailedRecording = new(2301, "MatchStatisticsRecordingFailed");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(CheckInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessPendingAsync(stoppingToken);
        }
    }

    internal async Task ProcessPendingAsync(CancellationToken cancellationToken)
    {
        foreach (CompletedMatch match in matches.GetPendingResults(16))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using IDisposable? diagnostics = logger.BeginScope(
                    new Dictionary<string, object> { [ServiceDispatchErrors.FailureOwnerScope] = true });
                await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                var recorder = scope.ServiceProvider.GetRequiredService<CompletedMatchRecorder>();
                await recorder.RecordAsync(match, cancellationToken);
                matches.AcknowledgeResult(match.MatchId);
            }
            catch (Exception exception) when (exception is DbException or DbUpdateException
                or IOException or UnauthorizedAccessException or TimeoutException)
            {
                matches.DeferResult(match.MatchId);
                Exception cause = exception.GetBaseException();
                long pendingSeconds = (long)Math.Max(0, (clock.GetUtcNow().UtcDateTime - match.CompletedAt).TotalSeconds);
                logger.LogWarning(FailedRecording,
                    "Could not record statistics for match {MatchId}. Pending for {PendingSeconds} seconds. "
                    + "Cause: {ErrorType} ({HResult}). Stack trace: {StackTrace}",
                    match.MatchId, pendingSeconds, cause.GetType().FullName, cause.HResult, cause.StackTrace);
                break;
            }
        }
    }
}
