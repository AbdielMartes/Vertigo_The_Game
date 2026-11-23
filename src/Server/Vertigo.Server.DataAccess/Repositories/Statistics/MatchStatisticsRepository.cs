using System.Data;
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Game;
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Statistics;

public sealed class MatchStatisticsRepository(VertigoDatabaseContext database)
{
    public async Task RecordAsync(MatchHistory match, IReadOnlyList<PlayerStats> contributions, CancellationToken cancellationToken)
    {
        await using var transaction = await database.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        if (!await database.MatchHistory.AnyAsync(row => row.MatchId == match.MatchId, cancellationToken))
        {
            Guid[] participants = contributions.Select(row => row.PlayerId).ToArray();
            var players = await database.Players.Include(player => player.Stats)
                .Where(player => participants.Contains(player.PlayerId))
                .ToDictionaryAsync(player => player.PlayerId, cancellationToken);
            if (players.Count > 0)
            {
                match.WinnerId = match.WinnerId is Guid winner && players.ContainsKey(winner) ? winner : null;
                database.MatchHistory.Add(match);
                foreach (PlayerStats score in contributions)
                {
                    if (players.TryGetValue(score.PlayerId, out Player? player))
                    {
                        UpdateStats(player, score);
                    }
                }

                await database.SaveChangesAsync(cancellationToken);
            }
        }

        await transaction.CommitAsync(cancellationToken);
    }

    private void UpdateStats(Player player, PlayerStats contribution)
    {
        if (player.Stats is null)
        {
            player.Stats = new PlayerStats { PlayerId = player.PlayerId };
            database.PlayerStats.Add(player.Stats);
        }

        PlayerStats stats = player.Stats;
        stats.MatchesPlayed++;
        stats.MatchesWon += contribution.MatchesWon;
        stats.TotalFloorsBuilt += contribution.TotalFloorsBuilt;
        stats.HighestSkyscraper = Math.Max(stats.HighestSkyscraper, contribution.HighestSkyscraper);
    }

    public List<RankingPlayer> GetRanking(int limit)
    {
        return database.PlayerStats.AsNoTracking().Where(stats => stats.MatchesPlayed > 0)
            .OrderByDescending(stats => stats.MatchesWon)
            .ThenByDescending(stats => stats.TotalFloorsBuilt)
            .ThenBy(stats => stats.Player.Username)
            .ThenBy(stats => stats.PlayerId)
            .Take(limit)
            .Select(stats => new RankingPlayer(stats.Player.Username, stats.MatchesWon, stats.TotalFloorsBuilt))
            .ToList();
    }

    public (int Played, int Won) GetRecord(Guid playerId)
    {
        var record = database.PlayerStats.AsNoTracking().Where(stats => stats.PlayerId == playerId)
            .Select(stats => new { stats.MatchesPlayed, stats.MatchesWon }).SingleOrDefault();
        return (record?.MatchesPlayed ?? 0, record?.MatchesWon ?? 0);
    }

    public sealed record RankingPlayer(string Username, int MatchesWon, int FloorsBuilt);
}
