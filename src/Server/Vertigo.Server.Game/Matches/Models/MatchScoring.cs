using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Models;

internal sealed class MatchScoring
{
    private const int HighestTowerBonus = 10;
    private readonly List<RoundScore> _rounds = new();

    public void AddRound(IReadOnlyDictionary<Guid, int> floors, Guid? closedBy)
    {
        int highest = floors.Values.Max();
        _rounds.Add(new RoundScore(new Dictionary<Guid, int>(floors), closedBy, highest));
    }

    public int GetGlobalFloors(Guid playerId)
    {
        return _rounds.Sum(round => round.Floors[playerId] + GetBonus(round, playerId));
    }

    public List<MatchRoundSummary> ToRoundSummaries()
    {
        List<MatchRoundSummary> summaries = new();
        Dictionary<Guid, int> totals = new();

        foreach (RoundScore round in _rounds)
        {
            MatchRoundSummary summary = new()
            {
                RoundNumber = summaries.Count + 1,
                ClosedByPlayerId = round.ClosedBy
            };
            foreach ((Guid playerId, int floors) in round.Floors)
            {
                int bonus = GetBonus(round, playerId);
                int total = totals.GetValueOrDefault(playerId) + floors + bonus;
                totals[playerId] = total;
                summary.Players.Add(new MatchRoundPlayerScore
                {
                    PlayerId = playerId,
                    FloorsBuilt = floors,
                    BonusFloors = bonus,
                    TotalFloors = total
                });
            }

            summaries.Add(summary);
        }

        return summaries;
    }

    public Guid[] GetWinners(IReadOnlyList<Guid> players)
    {
        (Guid PlayerId, (int GlobalFloors, int RoundsClosed, int HighestPersonalFloors) Rank)[] ranks =
            players.Select(playerId => (playerId, GetRank(playerId))).ToArray();
        (int GlobalFloors, int RoundsClosed, int HighestPersonalFloors) highest = ranks.Max(entry => entry.Rank);

        return ranks.Where(entry => entry.Rank == highest).Select(entry => entry.PlayerId).ToArray();
    }

    private (int GlobalFloors, int RoundsClosed, int HighestPersonalFloors) GetRank(Guid playerId)
    {
        return (GetGlobalFloors(playerId), _rounds.Count(round => round.ClosedBy == playerId),
            _rounds.Max(round => round.Floors[playerId]));
    }

    private static int GetBonus(RoundScore round, Guid playerId)
    {
        int floors = round.Floors[playerId];
        int bonus = floors > 0 && floors == round.Highest ? HighestTowerBonus : 0;

        return bonus;
    }

    private sealed record RoundScore(IReadOnlyDictionary<Guid, int> Floors, Guid? ClosedBy, int Highest);
}
