using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchMasterPlanTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UnixEpoch;

    [Fact]
    public void TryUseMasterPlan_ValidChain_ConsumesOnePlanAndPublishesOneMove()
    {
        MatchRecord match = CreateMatchWithEarnedPlan();
        MatchBoardSnapshot before = match.ToBoard();
        Guid playerId = before.CurrentTurnPlayerId;
        MatchMasterPlan plan = new(new[] { 42, 48 }, 1, before.Revision);

        MatchPlacementResult applied = match.TryUseMasterPlan(playerId, plan, Now);
        MatchPlacementResult repeated = match.TryUseMasterPlan(playerId, plan, Now);

        Assert.Equal(MatchPlacementResult.Applied, applied);
        Assert.Equal(MatchPlacementResult.StaleRevision, repeated);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(match.Session.PlayerIds[0], after.CurrentTurnPlayerId);
        Assert.Equal(48, after.Piles[0].TopCardNumber);
        Assert.Equal(3, after.Piles[0].CardCount);
        Assert.Equal(3, after.Players.Single(player => player.PlayerId == playerId).FloorsBuilt);
        MatchPrivateHand hand = match.ToPrivateHand(playerId)!;
        Assert.Equal(0, hand.MasterPlans);
        Assert.Equal(new[] { 43, 44, 45, 46, 47, 49, 50 }, hand.Cards);
        Assert.Equal(4, after.RecentMoves.Count);
        Assert.Equal(new[] { MatchBoardActionKind.Placement, MatchBoardActionKind.Dismantle,
            MatchBoardActionKind.Placement, MatchBoardActionKind.MasterPlan },
            after.RecentMoves.Select(entry => entry.ActionKind));
        MatchBoardMove move = after.RecentMoves[^1];
        Assert.Equal(after.Revision, move.Revision);
        Assert.Equal(playerId, move.PlayerId);
        Assert.Equal(36, move.PreviousTop);
        Assert.Equal(new[] { 42, 48 }, move.CardNumbers);

        move.CardNumbers.Clear();
        after.RecentMoves.Clear();
        Assert.Equal(new[] { 42, 48 }, match.ToBoard().RecentMoves[^1].CardNumbers);
    }

    [Theory]
    [InlineData(new[] { 42, 45, 44 }, 1, MatchPlacementResult.InvalidMasterPlanChain)]
    [InlineData(new[] { 42, 45, 42 }, 1, MatchPlacementResult.InvalidMasterPlanChain)]
    [InlineData(new[] { 42 }, 1, MatchPlacementResult.InvalidMasterPlanChain)]
    [InlineData(new[] { 42, 51 }, 1, MatchPlacementResult.CardNotOwned)]
    [InlineData(new[] { 42, 48 }, 5, MatchPlacementResult.InvalidPile)]
    public void TryUseMasterPlan_InvalidRequest_PreservesTurnPlanHandTowerAndHistory(
        int[] modules, int pile, MatchPlacementResult expected)
    {
        MatchRecord match = CreateMatchWithEarnedPlan();
        MatchBoardSnapshot before = match.ToBoard();
        Guid playerId = before.CurrentTurnPlayerId;
        MatchPrivateHand hand = match.ToPrivateHand(playerId)!;
        MatchMasterPlan plan = new(modules, pile, before.Revision);

        MatchPlacementResult result = match.TryUseMasterPlan(playerId, plan, Now);

        Assert.Equal(expected, result);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(before.Revision, after.Revision);
        Assert.Equal(before.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(before.Piles[0].CardCount, after.Piles[0].CardCount);
        Assert.Equal(before.Piles[0].TopCardNumber, after.Piles[0].TopCardNumber);
        Assert.Equal(before.RecentMoves.Select(move => move.Revision), after.RecentMoves.Select(move => move.Revision));
        MatchPrivateHand afterHand = match.ToPrivateHand(playerId)!;
        Assert.Equal(hand.Cards, afterHand.Cards);
        Assert.Equal(1, afterHand.MasterPlans);
    }

    [Fact]
    public void TryUseMasterPlan_EmptyHand_ClosesRoundAndKeepsMoveInHistory()
    {
        MatchRecord match = CreateMatchWithEarnedPlan();
        MatchBoardSnapshot before = match.ToBoard();
        Guid playerId = before.CurrentTurnPlayerId;
        int[] modules = Enumerable.Range(42, 9).ToArray();

        MatchPlacementResult result = match.TryUseMasterPlan(
            playerId, new MatchMasterPlan(modules, 1, before.Revision), Now);

        Assert.Equal(MatchPlacementResult.Applied, result);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(2, after.RoundNumber);
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(match.Session.PlayerIds[0], after.CurrentTurnPlayerId);
        MatchRoundSummary summary = Assert.Single(after.RoundSummaries);
        Assert.Equal(playerId, summary.ClosedByPlayerId);
        MatchRoundPlayerScore score = summary.Players.Single(entry => entry.PlayerId == playerId);
        Assert.Equal((3, 10, 13), (score.FloorsBuilt, score.BonusFloors, score.TotalFloors));
        Assert.Equal(modules, after.RecentMoves[^1].CardNumbers);
        Assert.Equal(0, match.ToPrivateHand(playerId)!.MasterPlans);
        Assert.Equal(10, match.ToPrivateHand(playerId)!.Cards.Count);
    }

    private static MatchRecord CreateMatchWithEarnedPlan()
    {
        Guid[] players = { Guid.NewGuid(), Guid.NewGuid() };
        MatchSessionSnapshot session = new()
        {
            MatchId = Guid.NewGuid(),
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            InitialHostPlayerId = players[0],
            PlayerIds = players,
            CreatedAtUtc = Now.UtcDateTime
        };
        int[] modules = { 4, 81, 91, 20, 21, 22, 23, 24, 25, 26,
            3, 42, 43, 44, 45, 46, 47, 48, 49, 50, 1, 80, 90, 100, 36, 99 };
        MatchRecord match = new(session, Now, () => new MatchDeck(modules));
        if (match.ToBoard().CurrentTurnPlayerId == players[0])
        {
            Place(match, 81, 2);
        }

        MatchBoardSnapshot before = match.ToBoard();
        Assert.Equal(MatchPlacementResult.MasterPlanRequired, match.TryUseMasterPlan(
            players[1], new MatchMasterPlan(new[] { 42, 48 }, 1, before.Revision), Now));
        Place(match, 3, 1);
        Place(match, 4, 1);
        MatchBoardSnapshot towerReady = match.ToBoard();
        Assert.Equal(MatchDismantleResult.Applied, match.TryDismantle(
            players[1], new MatchDismantle(1, towerReady.Revision), Now));
        Place(match, 91, 3);

        return match;
    }

    private static void Place(MatchRecord match, int module, int position)
    {
        MatchBoardSnapshot board = match.ToBoard();
        MatchPlacement placement = new(module, position, board.Revision);

        Assert.Equal(MatchPlacementResult.Applied, match.TryPlace(board.CurrentTurnPlayerId, placement, Now));
    }
}
