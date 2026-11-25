using System.Data;
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Enums;
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

    public List<RankingPlayer> GetRanking(int limit, Guid? viewerId = null)
    {
        return ReadRanking(VisiblePlayers(viewerId)
            .Where(player => player.Stats != null && player.Stats.MatchesPlayed > 0), limit);
    }

    public List<RankingPlayer> GetFriendsRanking(Guid playerId)
    {
        return ReadRanking(VisiblePlayers(playerId).Where(player =>
            player.PlayerId == playerId || database.Friendships.Any(friendship =>
                friendship.Status == FriendshipStatus.Accepted
                && ((friendship.PlayerRequesterId == playerId && friendship.PlayerAddresseeId == player.PlayerId)
                    || (friendship.PlayerAddresseeId == playerId && friendship.PlayerRequesterId == player.PlayerId)))));
    }

    public List<RankingPlayer> GetCountryRanking(Guid playerId, int limit)
    {
        return ReadRanking(VisiblePlayers(playerId).Where(player =>
            player.CountryCode != null && player.Stats != null && player.Stats.MatchesPlayed > 0
            && database.Players.Any(viewer => viewer.PlayerId == playerId && viewer.CountryCode == player.CountryCode)),
            limit);
    }

    private static List<RankingPlayer> ReadRanking(IQueryable<Player> players, int? limit = null)
    {
        IQueryable<Player> ranking = players
            .OrderByDescending(player => player.Stats == null ? 0 : player.Stats.MatchesWon)
            .ThenByDescending(player => player.Stats == null ? 0 : player.Stats.TotalFloorsBuilt)
            .ThenBy(player => player.Username)
            .ThenBy(player => player.PlayerId);
        if (limit.HasValue)
        {
            ranking = ranking.Take(limit.Value);
        }

        return ranking.Select(player => new RankingPlayer(player.PlayerId, player.Username, player.AvatarId,
                player.Stats == null ? 0 : player.Stats.MatchesWon,
                player.Stats == null ? 0 : player.Stats.TotalFloorsBuilt))
            .ToList();
    }

    private IQueryable<Player> VisiblePlayers(Guid? viewerId)
    {
        return database.Players.AsNoTracking().Where(player => !database.BlockedPlayers.Any(block =>
            block.PlayerBlockerId == player.PlayerId && block.PlayerBlockedId == viewerId));
    }

    public PlayerProfileRecord? GetPlayerProfile(Guid playerId, Guid? viewerId = null)
    {
        return VisiblePlayers(viewerId)
            .Where(player => player.PlayerId == playerId)
            .Select(player => new PlayerProfileRecord(
                player.Username,
                player.CreatedAt.Year,
                player.AvatarId,
                player.CountryCode,
                player.Stats == null ? 0 : player.Stats.MatchesPlayed,
                player.Stats == null ? 0 : player.Stats.MatchesWon))
            .SingleOrDefault();
    }

    public sealed record RankingPlayer(Guid PlayerId, string Username, int? AvatarId, int MatchesWon, int FloorsBuilt);
}
