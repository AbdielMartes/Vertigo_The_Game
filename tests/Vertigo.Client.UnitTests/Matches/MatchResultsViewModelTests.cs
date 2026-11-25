using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchResultsViewModelTests
{
    [Fact]
    public void ApplyBoard_RoundCloses_OpensPreviewThenCollapsesAndCanBeReopened()
    {
        PreviewClock clock = new();
        MatchResultsViewModel results = new(new ResultsText(), clock);
        MatchRoundSummary round = CreateRound(1, (5, 10), 15);
        MatchBoardSnapshot board = new() { RoundNumber = 2, RoundSummaries = { round } };

        results.ApplyBoard(board);

        Assert.True(results.HasResults);
        Assert.False(results.HasFinalResult);
        Assert.True(results.IsRoundSummaryOpen);
        Assert.Equal("Ronda 1 finalizada", results.HeaderText);
        MatchRoundPlayerScore score = Assert.Single(results.SelectedScores);
        Assert.Equal("Ana", score.PlayerUsername);
        Assert.Equal((5, 10, 15), (score.FloorsBuilt, score.BonusFloors, score.TotalFloors));

        clock.Advance(TimeSpan.FromSeconds(6));
        results.UpdateRoundPreview();
        Assert.True(results.IsRoundSummaryOpen);
        results.ApplyBoard(board);
        clock.Advance(TimeSpan.FromSeconds(1));
        results.UpdateRoundPreview();
        Assert.False(results.IsRoundSummaryOpen);
        Assert.True(results.ShowRoundSummary);

        results.ToggleRoundSummary();
        clock.Advance(TimeSpan.FromSeconds(7));
        results.UpdateRoundPreview();
        Assert.True(results.IsRoundSummaryOpen);
        results.ToggleRoundSummary();
        Assert.False(results.IsRoundSummaryOpen);
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

        results.ApplyBoard(board, ana);

        Assert.True(results.HasFinalResult);
        Assert.True(results.IsExpanded);
        Assert.False(results.IsAbandonment);
        Assert.Equal("Ganaron Ana, Beto", results.WinnerText);
        Assert.Equal("Match.Results.SharedVictory", results.ResultTitle);
        Assert.True(results.IsLocalWinner);
        Assert.All(results.FinalStandings, row => Assert.True(row.IsWinner));
        results.IsExpanded = false;
        results.ApplyBoard(board, beto);
        Assert.False(results.IsExpanded);
        Assert.Equal("Beto", Assert.Single(results.FinalStandings, row => row.IsLocal).Username);
    }

    [Fact]
    public void ApplyBoard_AbandonmentBeforeFirstRound_ShowsReasonWithoutWinnerOrScores()
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
        Assert.True(results.IsEarlyAbandonment);
        Assert.Equal("Match.Results.NoWinner", results.ResultTitle);
        Assert.Equal("Match.Results.EarlyAbandonmentDetail", results.ResultDetail);
        Assert.Empty(results.WinnerText);
        Assert.Empty(results.SelectedScores);
        results.Reset();
        Assert.False(results.HasResults);
        Assert.False(results.IsExpanded);
        Assert.Empty(results.WinnerText);
    }

    [Fact]
    public void ApplyBoard_LatestTowerStaysVisibleWhileAnEarlierRoundIsSelected()
    {
        Guid local = Guid.NewGuid();
        MatchResultsViewModel results = new(new ResultsText());
        MatchRoundSummary first = CreateRound(1, (5, 10), 15);
        MatchRoundSummary second = CreateRound(2, (4, 0), 19);
        first.Players[0].PlayerId = local;
        second.Players[0].PlayerId = local;
        results.ApplyBoard(new MatchBoardSnapshot { RoundSummaries = { first, second } }, local);
        results.SelectedRound = results.Rounds.Single(round => round.RoundNumber == 1);

        Assert.True(results.ShowLatestTower);
        Assert.True(results.IsRoundSummaryOpen);
        Assert.Equal(4, results.LatestTower!.BuiltFloors);
        Assert.Equal(10, results.SelectedTower!.BonusFloors);
        Assert.False(Assert.Single(results.SelectedTower.Segments).IsBonus);
        Assert.Equal(15, results.SelectedTower.TotalFloors);
    }

    private sealed class PreviewClock : TimeProvider
    {
        private long _timestamp;

        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public override long GetTimestamp() => _timestamp;

        public void Advance(TimeSpan duration)
        {
            _timestamp += duration.Ticks;
        }
    }

    [Fact]
    public void ApplyBoard_FinalStandingUsesOfficialPositionsAndOnlyCreditedFloors()
    {
        Guid ana = Guid.NewGuid();
        Guid beto = Guid.NewGuid();
        MatchRoundSummary round = CreateRound(1, (5, 10), 15);
        round.Players[0].PlayerId = ana;
        MatchBoardSnapshot board = new()
        {
            EndReason = MatchEndReason.Completed,
            WinnerIds = { beto },
            RoundSummaries = { round },
            Players =
            {
                new MatchBoardPlayer { PlayerId = ana, Username = "Ana", GlobalFloors = 15, FloorsBuilt = 99, FinalPosition = 2 },
                new MatchBoardPlayer { PlayerId = beto, Username = "Beto", GlobalFloors = 15, FinalPosition = 1 }
            }
        };

        MatchResultsViewModel results = new(new ResultsText());
        results.ApplyBoard(board, ana);

        Assert.True(results.IsExpanded);
        Assert.False(results.ShowLatestTower);
        Assert.Equal(new int?[] { 1, 2 }, results.FinalStandings.Select(row => row.Position));
        Assert.True(results.FinalStandings[0].IsWinner);
        Assert.Equal("Match.Results.Defeat", results.ResultTitle);
        Assert.Equal("Ganó Beto", results.ResultDetail);
        Assert.Equal("Ana", Assert.Single(results.FinalStandings, row => row.IsLocal).Username);
        Assert.Equal(5, results.FinalTower!.BuiltFloors);
        Assert.Equal(10, results.FinalTower.BonusFloors);
        Assert.Equal(15, results.FinalTower.TotalFloors);
        Assert.Contains(results.FinalTower.Segments, segment => segment.IsBonus);
        results.ApplyBoard(board, beto);
        Assert.Equal("Match.Results.Victory", results.ResultTitle);
        Assert.True(results.IsLocalWinner);
        results.ApplyBoard(board, Guid.NewGuid());
        Assert.Equal("Match.Results.Finished", results.ResultTitle);
        Assert.False(results.IsLocalDefeat);
        board.Players[0].FinalPosition = null;
        results.ApplyBoard(board, ana);
        Assert.Equal("Match.Results.Finished", results.ResultTitle);
        Assert.False(results.IsLocalDefeat);
    }

    [Fact]
    public void ApplyBoard_AbandonmentWithoutClosedRounds_DoesNotCreditOpenRound()
    {
        Guid winner = Guid.NewGuid();
        MatchResultsViewModel results = new(new ResultsText());
        results.ApplyBoard(new MatchBoardSnapshot
        {
            EndReason = MatchEndReason.Abandonment,
            WinnerIds = { winner },
            Players = { new MatchBoardPlayer { PlayerId = winner, FloorsBuilt = 12, FinalPosition = 1 } }
        }, winner);

        Assert.Equal("Match.Results.NoWinner", results.ResultTitle);
        Assert.Null(results.FinalTower);
        var survivor = Assert.Single(results.FinalStandings);
        Assert.Equal(0, survivor.TotalFloors);
        Assert.Null(survivor.Position);
        Assert.False(survivor.IsWinner);
        Assert.True(survivor.IsLocal);
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
