using Microsoft.Extensions.Hosting;
using Vertigo.Server.Matchmaking.Lobbies.Services;

namespace Vertigo.Server.Host.Matches;

internal sealed class MatchTurnClockWorker : BackgroundService
{
    private static readonly TimeSpan CheckInterval = TimeSpan.FromMilliseconds(200);
    private readonly LobbyService _lobbies;
    private readonly TimeProvider _timeProvider;

    public MatchTurnClockWorker(LobbyService lobbies, TimeProvider timeProvider)
    {
        _lobbies = lobbies;
        _timeProvider = timeProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(CheckInterval, _timeProvider);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            _lobbies.ProcessExpiredMatchTurns();
        }
    }
}
