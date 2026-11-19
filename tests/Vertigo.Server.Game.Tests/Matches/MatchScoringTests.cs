using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchScoringTests
{
    [Fact]
    public void ToRoundSummaries_MultipleRounds_ReportsBonusAndRunningTotalsWithoutSharingState()
    {
        MatchScoring scoring = new();
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 5, [second] = 5 }, first);
        scoring.AddRound(new Dictionary<Guid, int> { [first] = 2, [second] = 0 }, null);

        List<MatchRoundSummary> summaries = scoring.ToRoundSummaries();

        Assert.Equal(new[] { 1, 2 }, summaries.Select(round => round.RoundNumber));
        Assert.Equal(first, summaries[0].ClosedByPlayerId);
        Assert.Null(summaries[1].ClosedByPlayerId);
        MatchRoundPlayerScore firstRound = summaries[0].Players.Single(score => score.PlayerId == first);
        MatchRoundPlayerScore secondRound = summaries[1].Players.Single(score => score.PlayerId == first);
        MatchRoundPlayerScore otherScore = summaries[1].Players.Single(score => score.PlayerId == second);
        Assert.Equal((5, 10, 15), (firstRound.FloorsBuilt, firstRound.BonusFloors, firstRound.TotalFloors));
        Assert.Equal((2, 10, 27), (secondRound.FloorsBuilt, secondRound.BonusFloors, secondRound.TotalFloors));
        Assert.Equal((0, 0, 15), (otherScore.FloorsBuilt, otherScore.BonusFloors, otherScore.TotalFloors));

        firstRound.TotalFloors = 999;
        summaries[1].Players.Clear();
        List<MatchRoundSummary> refreshed = scoring.ToRoundSummaries();
        Assert.Equal(15, refreshed[0].Players.Single(score => score.PlayerId == first).TotalFloors);
        Assert.Equal(2, refreshed[1].Players.Count);
        Assert.Equal(27, scoring.GetGlobalFloors(first));
    }

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
