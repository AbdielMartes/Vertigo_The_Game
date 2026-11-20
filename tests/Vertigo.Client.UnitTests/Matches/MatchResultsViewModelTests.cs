using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchResultsViewModelTests
{
    [Fact]
    public void ApplyBoard_RoundCloses_OffersScoresWithoutOpeningPanel()
    {
        MatchResultsViewModel results = new(new ResultsText());
        MatchRoundSummary round = CreateRound(1, (5, 10), 15);

        results.ApplyBoard(new MatchBoardSnapshot { RoundNumber = 2, RoundSummaries = { round } });

        Assert.True(results.HasResults);
        Assert.False(results.HasFinalResult);
        Assert.False(results.IsExpanded);
        Assert.Equal("Ronda 1 finalizada", results.HeaderText);
        MatchRoundPlayerScore score = Assert.Single(results.SelectedScores);
        Assert.Equal("Ana", score.PlayerUsername);
        Assert.Equal((5, 10, 15), (score.FloorsBuilt, score.BonusFloors, score.TotalFloors));
    }

    [Fact]
    public void ApplyBoard_UnchangedRounds_KeepsTheSelectedEarlierRound()
    {
        MatchResultsViewModel results = new(new ResultsText());
        MatchRoundSummary first = CreateRound(1, (5, 10), 15);
        MatchRoundSummary second = CreateRound(2, (4, 0), 19);
        MatchBoardSnapshot board = new() { RoundNumber = 3, RoundSummaries = { first, second } };
        results.ApplyBoard(board);
        Assert.Equal(2, results.SelectedRound?.RoundNumber);
        results.SelectedRound = results.Rounds.Single(row => row.RoundNumber == 1);

        results.ApplyBoard(board);
        results.RefreshLocalization();

        Assert.Equal(1, results.SelectedRound?.RoundNumber);
        Assert.Equal(15, Assert.Single(results.SelectedScores).TotalFloors);
    }

    [Fact]
    public void ApplyBoard_NewRoundWhileReviewingEarlierRound_PreservesSelection()
    {
        MatchResultsViewModel results = new(new ResultsText());
        MatchRoundSummary first = CreateRound(1, (5, 10), 15);
        MatchRoundSummary second = CreateRound(2, (4, 0), 19);
        MatchRoundSummary third = CreateRound(3, (2, 0), 21);
        results.ApplyBoard(new MatchBoardSnapshot { RoundSummaries = { first, second } });
        results.SelectedRound = results.Rounds.Single(row => row.RoundNumber == 1);

        results.ApplyBoard(new MatchBoardSnapshot { RoundSummaries = { first, second, third } });

        Assert.Equal(1, results.SelectedRound?.RoundNumber);
        Assert.Equal(15, Assert.Single(results.SelectedScores).TotalFloors);
        Assert.Equal(3, results.Rounds.Count);
    }

    [Fact]
    public void ApplyBoard_CompletedWithJointWinners_OpensTheFinalResultOnce()
    {
        MatchResultsViewModel results = new(new ResultsText());
        Guid ana = Guid.NewGuid();
        Guid beto = Guid.NewGuid();
        MatchBoardSnapshot board = new()
        {
            EndReason = MatchEndReason.Completed,
            WinnerIds = { ana, beto },
            Players =
            {
                new MatchBoardPlayer { PlayerId = ana, Username = "Ana" },
                new MatchBoardPlayer { PlayerId = beto, Username = "Beto" }
            }
        };

        results.ApplyBoard(board);

        Assert.True(results.HasFinalResult);
        Assert.True(results.IsExpanded);
        Assert.False(results.IsAbandonment);
        Assert.Equal("Ganaron Ana, Beto", results.WinnerText);
        results.IsExpanded = false;
        results.ApplyBoard(board);
        Assert.False(results.IsExpanded);
    }

    [Fact]
    public void ApplyBoard_AbandonmentBeforeFirstRound_ShowsWinnerAndReasonWithoutScores()
    {
        MatchResultsViewModel results = new(new ResultsText());
        Guid winner = Guid.NewGuid();

        results.ApplyBoard(new MatchBoardSnapshot
        {
            EndReason = MatchEndReason.Abandonment,
            WinnerIds = { winner },
            Players = { new MatchBoardPlayer { PlayerId = winner, Username = "Ana" } }
        });

        Assert.True(results.HasResults);
        Assert.True(results.IsExpanded);
        Assert.True(results.IsAbandonment);
        Assert.Equal("Ganó Ana", results.WinnerText);
        Assert.Empty(results.SelectedScores);
        results.Reset();
        Assert.False(results.HasResults);
        Assert.False(results.IsExpanded);
        Assert.Empty(results.WinnerText);
    }

    private static MatchRoundSummary CreateRound(int number, (int Built, int Bonus) floors, int total)
    {
        return new MatchRoundSummary
        {
            RoundNumber = number,
            Players =
            {
                new MatchRoundPlayerScore
                {
                    PlayerUsername = "Ana",
                    FloorsBuilt = floors.Built,
                    BonusFloors = floors.Bonus,
                    TotalFloors = total
                }
            }
        };
    }

    private sealed class ResultsText : ITextLocalizer
    {
        public string Get(string key)
        {
            return key switch
            {
                "Match.Results.RoundClosed" => "Ronda {0} finalizada",
                "Match.Results.RoundOption" => "Ronda {0}",
                "Match.Results.Winner" => "Ganó {0}",
                "Match.Results.Winners" => "Ganaron {0}",
                _ => key
            };
        }
    }
}
