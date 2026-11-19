using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Contracts.Models;
using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Client.UI.ViewModels.Presentation;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchTurnPresentationTests
{
    private static readonly DateTimeOffset ServerTime = DateTimeOffset.UnixEpoch;

    [Fact]
    public void GetRemainingSeconds_WithServerSample_IgnoresLocalWallClockAndClampsAtZero()
    {
        ManualClock clock = new();
        MatchTurnClock countdown = new(clock);
        countdown.SetDeadline(ServerTime.AddSeconds(20), ServerTime);

        Assert.Equal(20, countdown.GetRemainingSeconds());
        clock.MoveBy(TimeSpan.FromMilliseconds(600));
        Assert.Equal(20, countdown.GetRemainingSeconds());
        clock.MoveBy(TimeSpan.FromMilliseconds(400));
        Assert.Equal(19, countdown.GetRemainingSeconds());
        clock.MoveBy(TimeSpan.FromSeconds(25));
        Assert.Equal(0, countdown.GetRemainingSeconds());
        countdown.SetDeadline(null, null);
        Assert.Null(countdown.GetRemainingSeconds());
    }

    [Fact]
    public void GetRemainingSeconds_AfterReconnect_UsesRemainingServerTime()
    {
        ManualClock clock = new();
        MatchTurnClock countdown = new(clock);

        countdown.SetDeadline(ServerTime.AddSeconds(20), ServerTime.AddSeconds(14));

        Assert.Equal(6, countdown.GetRemainingSeconds());
    }

    [Fact]
    public void HasLegalPlacement_OnlyAnotherTowerAcceptsCard_ReturnsTrue()
    {
        int?[] tops = { 80, null, 12 };
        int[] hand = { 15, 55 };

        Assert.False(MatchTurnRules.CanPlace(tops[0], hand[0]));
        Assert.True(MatchTurnRules.HasLegalPlacement(tops, hand));
    }

    [Fact]
    public void HasLegalPlacement_NoTowerAcceptsAnyCard_ReturnsFalse()
    {
        int?[] tops = { 80, null, 12 };
        int[] hand = { 10, 40, 100 };

        Assert.False(MatchTurnRules.HasLegalPlacement(tops, hand));
    }

    [Theory]
    [InlineData(12, 12, false)]
    [InlineData(12, 13, true)]
    [InlineData(12, 22, true)]
    [InlineData(12, 23, false)]
    [InlineData(95, 101, false)]
    [InlineData(null, 15, false)]
    public void CanPlace_ModuleAndTower_UsesGameLimits(int? top, int module, bool expected)
    {
        Assert.Equal(expected, MatchTurnRules.CanPlace(top, module));
    }

    [Fact]
    public void ApplyBoardAndHand_LegalCardOnAnotherTower_EnablesOnlyItsPlacement()
    {
        TurnFixture turn = CreateTurn();
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        turn.Model.SelectedCard = Assert.Single(turn.Model.Hand);

        turn.Model.SelectedPile = turn.Model.Piles[0];
        Assert.False(turn.Model.PlaceCardCommand.CanExecute(null));
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));

        turn.Model.SelectedPile = turn.Model.Piles[1];
        Assert.True(turn.Model.PlaceCardCommand.CanExecute(null));
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));
    }

    [Fact]
    public void ApplyBoardAndHand_BlockedHand_RequiresANonemptyTowerAndUnexpiredTurn()
    {
        TurnFixture turn = CreateTurn();
        turn.Hand.Cards = new List<int> { 50 };
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        turn.Model.SelectedPile = turn.Model.Piles[2];
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));
        turn.Model.SelectedPile = turn.Model.Piles[0];
        Assert.True(turn.Model.DismantleTowerCommand.CanExecute(null));
        turn.Clock.MoveBy(TimeSpan.FromSeconds(20));
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));
        Assert.False(turn.Model.PlaceCardCommand.CanExecute(null));
    }

    [Fact]
    public void ApplyBoardAndHand_HandFromDifferentRevision_DisablesActionsUntilMatchingHandArrives()
    {
        TurnFixture turn = CreateTurn();
        turn.Hand.Revision = turn.Board.Revision + 1;

        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.Empty(turn.Model.Hand);
        turn.Model.SelectedPile = turn.Model.Piles[0];
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));

        turn.Board.Revision++;
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        turn.Model.SelectedCard = Assert.Single(turn.Model.Hand);
        turn.Model.SelectedPile = turn.Model.Piles[1];
        Assert.True(turn.Model.PlaceCardCommand.CanExecute(null));
    }

    [Fact]
    public void ApplyBoardAndHand_TurnChangesAndLocalPlayerIsExpelled_ClearsOldSelectionsAndDisablesActions()
    {
        TurnFixture turn = CreateTurn();
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        MatchCardRow oldCard = Assert.Single(turn.Model.Hand);
        MatchPileRow oldPile = turn.Model.Piles[1];
        turn.Model.SelectedCard = oldCard;
        turn.Model.SelectedPile = oldPile;
        Assert.True(turn.Model.PlaceCardCommand.CanExecute(null));

        turn.Board.Revision++;
        turn.Board.CurrentTurnPlayerId = turn.Board.Players[1].PlayerId;
        turn.Hand.Revision = turn.Board.Revision;
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        Assert.Null(turn.Model.SelectedCard);
        Assert.Null(turn.Model.SelectedPile);
        turn.Model.SelectedCard = oldCard;
        turn.Model.SelectedPile = oldPile;
        Assert.False(turn.Model.PlaceCardCommand.CanExecute(null));
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));

        turn.Board.Revision++;
        turn.Board.ExpelledPlayerIds.Add(turn.Hand.PlayerId);
        turn.Model.ApplyBoardAndHand(turn.Board, null);
        Assert.Empty(turn.Model.Hand);
        Assert.False(turn.Model.DismantleTowerCommand.CanExecute(null));
    }

    [Fact]
    public void ApplyBoardAndHand_SameRevisionAfterReconnect_ResamplesRemainingTime()
    {
        TurnFixture turn = CreateTurn();
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        turn.Clock.MoveBy(TimeSpan.FromSeconds(5));
        turn.Board.ServerTimeUtc = ServerTime.AddSeconds(14);

        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.Equal("6", turn.Model.TurnSecondsLabel);
    }

    [Fact]
    public void ApplyBoardAndHand_SlowPrivateHandRequest_DoesNotExtendCountdown()
    {
        TurnFixture turn = CreateTurn();
        long boardReceivedAt = turn.Clock.GetTimestamp();
        turn.Clock.MoveBy(TimeSpan.FromSeconds(4));

        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand, boardReceivedAt);

        Assert.Equal("16", turn.Model.TurnSecondsLabel);
    }

    [Fact]
    public void MasterPlanDraft_PendingVoteAndUnchangedHand_PreservesChain()
    {
        TurnFixture turn = CreateTurnWithDraft();

        turn.Board.Revision++;
        turn.Hand.Revision = turn.Board.Revision;
        turn.Board.ExpulsionVotes.Add(new MatchExpulsionVoteSummary { TargetPlayerId = Guid.NewGuid(), VoteCount = 1 });
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.Equal("12 → 15 → 22", turn.Model.MasterPlanPreview);
        Assert.True(turn.Model.PlayMasterPlanCommand.CanExecute(null));
        turn.Model.SelectedCard = turn.Model.Hand[0];
        Assert.False(turn.Model.PlaceCardCommand.CanExecute(null));
    }

    [Fact]
    public void MasterPlanDraft_ChangedTower_ClearsChainAndRejectsOldSelection()
    {
        TurnFixture turn = CreateTurnWithDraft();
        MatchPileRow oldPile = turn.Model.Piles[1];
        turn.Board.Revision++;
        turn.Hand.Revision = turn.Board.Revision;
        turn.Board.Piles[1].TopCardNumber = 14;

        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.Null(turn.Model.SelectedPile);
        Assert.False(turn.Model.PlayMasterPlanCommand.CanExecute(null));
        turn.Model.SelectedPile = oldPile;
        turn.Model.SelectedCard = turn.Model.Hand[0];
        Assert.False(turn.Model.AddMasterPlanModuleCommand.CanExecute(null));
    }

    [Fact]
    public void MasterPlanDraft_MissingModule_ClearsDraftAndRejectsOldCard()
    {
        TurnFixture turn = CreateTurnWithDraft();
        MatchCardRow oldCard = turn.Model.Hand[0];
        turn.Board.Revision++;
        turn.Hand.Revision = turn.Board.Revision;
        turn.Hand.Cards.Remove(15);

        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.False(turn.Model.PlayMasterPlanCommand.CanExecute(null));
        Assert.False(turn.Model.RemoveMasterPlanModuleCommand.CanExecute(null));
        turn.Model.SelectedCard = oldCard;
        Assert.False(turn.Model.AddMasterPlanModuleCommand.CanExecute(null));
    }

    [Fact]
    public void MasterPlanDraft_ExpiredTurn_DisablesPlayAndClearsDraftOnRefresh()
    {
        TurnFixture turn = CreateTurnWithDraft();
        turn.Clock.MoveBy(TimeSpan.FromSeconds(20));

        Assert.False(turn.Model.PlayMasterPlanCommand.CanExecute(null));
        turn.Board.ServerTimeUtc = turn.Board.TurnDeadlineUtc;
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);

        Assert.False(turn.Model.RemoveMasterPlanModuleCommand.CanExecute(null));
        Assert.False(turn.Model.PlayMasterPlanCommand.CanExecute(null));
    }

    private static TurnFixture CreateTurnWithDraft()
    {
        TurnFixture turn = CreateTurn();
        turn.Hand.MasterPlans = 1;
        turn.Hand.Cards.Add(22);
        turn.Model.ApplyBoardAndHand(turn.Board, turn.Hand);
        turn.Model.SelectedPile = turn.Model.Piles[1];

        foreach (MatchCardRow card in turn.Model.Hand)
        {
            turn.Model.SelectedCard = card;
            Assert.True(turn.Model.AddMasterPlanModuleCommand.CanExecute(null));
            turn.Model.AddMasterPlanModuleCommand.Execute(null);
        }

        Assert.True(turn.Model.PlayMasterPlanCommand.CanExecute(null));

        return turn;
    }

    private static TurnFixture CreateTurn()
    {
        ManualClock clock = new();
        PlayerSession session = new(clock);
        Guid player = Guid.NewGuid();
        session.SetSession(new SessionSnapshot(player, "Ana", "test-token", clock.GetUtcNow().UtcDateTime.AddHours(1),
            true));
        Guid matchId = Guid.NewGuid();
        MatchViewModel model = new(new TestText(), session, clock);
        model.Activate(matchId, "ABC234");
        MatchBoardSnapshot board = new()
        {
            MatchId = matchId,
            Revision = 5,
            CurrentTurnPlayerId = player,
            ServerTimeUtc = ServerTime,
            TurnDeadlineUtc = ServerTime.AddSeconds(20),
            Players = { new MatchBoardPlayer { PlayerId = player }, new MatchBoardPlayer { PlayerId = Guid.NewGuid() } },
            Piles =
            {
                new MatchBoardPile { Position = 1, TopCardNumber = 80 },
                new MatchBoardPile { Position = 2, TopCardNumber = 12 },
                new MatchBoardPile { Position = 3 }
            }
        };
        MatchPrivateHand hand = new() { MatchId = matchId, PlayerId = player, Revision = 5, Cards = { 15 } };

        return new TurnFixture(model, board, hand, clock);
    }

    private sealed record TurnFixture(
        MatchViewModel Model, MatchBoardSnapshot Board, MatchPrivateHand Hand, ManualClock Clock);

    private sealed class TestText : ITextLocalizer
    {
        public string Get(string key)
        {
            return key == "Match.Turn.Seconds" ? "{0}" : key;
        }
    }

    private sealed class ManualClock : TimeProvider
    {
        private long _ticks;

        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public override DateTimeOffset GetUtcNow()
        {
            return ServerTime.AddHours(1);
        }

        public override long GetTimestamp()
        {
            return _ticks;
        }

        public void MoveBy(TimeSpan elapsed)
        {
            _ticks += elapsed.Ticks;
        }
    }
}
