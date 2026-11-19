using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchScoringTests
{
    [Fact]
    public void AddRound_TiedTallestPlayers_AwardsBothBonusesAndAllowsSharedWin()
    {
        MatchScoring scoring = new();
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();

        scoring.AddRound(new Dictionary<Guid, int> { [first] = 5, [second] = 5, [third] = 3 }, null);

        Assert.Equal(15, scoring.GetGlobalFloors(first));
        Assert.Equal(15, scoring.GetGlobalFloors(second));
        Assert.Equal(3, scoring.GetGlobalFloors(third));
        Assert.Equal(new[] { first, second }, scoring.GetWinners(new[] { first, second, third }));
    }

    [Fact]
    public void GetWinners_TiedGlobalScores_PrefersRoundClosuresBeforeConstructionHeight()
    {
        MatchScoring scoring = new();
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 8, [second] = 1 }, null);
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 0, [second] = 7 }, second);

        Assert.Equal(scoring.GetGlobalFloors(first), scoring.GetGlobalFloors(second));
        Assert.Equal(second, Assert.Single(scoring.GetWinners(new[] { first, second })));
    }

    [Fact]
    public void GetWinners_TiedGlobalScoresAndClosures_PrefersHighestPersonalTower()
    {
        MatchScoring scoring = new();
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 8, [second] = 1 }, first);
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 0, [second] = 7 }, second);
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 0, [second] = 0 }, null);

        Assert.Equal(18, scoring.GetGlobalFloors(first));
        Assert.Equal(18, scoring.GetGlobalFloors(second));
        Assert.Equal(first, Assert.Single(scoring.GetWinners(new[] { first, second })));
    }
}
