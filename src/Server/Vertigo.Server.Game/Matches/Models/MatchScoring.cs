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
        return _rounds.Sum(round => GetRoundScore(round, playerId));
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

    private static int GetRoundScore(RoundScore round, Guid playerId)
    {
        int floors = round.Floors[playerId];
        int bonus = floors > 0 && floors == round.Highest ? HighestTowerBonus : 0;

        return floors + bonus;
    }

    private sealed record RoundScore(IReadOnlyDictionary<Guid, int> Floors, Guid? ClosedBy, int Highest);
}
