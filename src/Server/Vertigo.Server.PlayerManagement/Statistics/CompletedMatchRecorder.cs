using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Entities.Game;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Statistics;

namespace Vertigo.Server.PlayerManagement.Statistics;

public sealed class CompletedMatchRecorder(MatchStatisticsRepository repository)
{
    public Task RecordAsync(CompletedMatch result, CancellationToken cancellationToken)
    {
        MatchHistory history = new()
        {
            MatchId = result.MatchId,
            PlayedAt = result.CompletedAt,
            RoundsPlayed = result.Rounds,
            WinningHeight = result.WinningHeight,
            WinnerId = result.SoleWinner
        };
        PlayerStats[] contributions = result.Players.Select(player => new PlayerStats
        {
            PlayerId = player.PlayerId,
            MatchesWon = player.Won ? 1 : 0,
            TotalFloorsBuilt = player.FloorsBuilt,
            HighestSkyscraper = player.HighestSkyscraper
        }).ToArray();
        return repository.RecordAsync(history, contributions, cancellationToken);
    }
}
