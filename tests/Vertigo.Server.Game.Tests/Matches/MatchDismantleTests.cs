using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchDismantleTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.UnixEpoch;

    [Fact]
    public void TryDismantle_PlacementOnAnotherTower_RejectsWithoutChangingState()
    {
        MatchRecord match = Create(
            Enumerable.Range(1, 10).Select(module => module * 2),
            Enumerable.Range(1, 10).Select(module => module * 2 + 1),
            [80, 90, 1, 70]);
        Dismantle(match, 1);
        MatchBoardSnapshot board = match.ToBoard();
        Guid active = board.CurrentTurnPlayerId;
        MatchPrivateHand hand = match.ToPrivateHand(active)!;

        MatchDismantleResult result = match.TryDismantle(active, new MatchDismantle(1, board.Revision), Now);

        Assert.Equal(MatchDismantleResult.PlacementAvailable, result);
        Assert.Equal(board.Revision, match.ToBoard().Revision);
        Assert.Equal(active, match.ToBoard().CurrentTurnPlayerId);
        Assert.Equal(90, match.ToBoard().Piles[0].TopCardNumber);
        Assert.Equal(board.Players.Select(player => player.FloorsBuilt),
            match.ToBoard().Players.Select(player => player.FloorsBuilt));
        Assert.Equal(hand.Cards, match.ToPrivateHand(active)!.Cards);
        Assert.Equal(0, match.ToPrivateHand(active)!.MasterPlans);
    }

    [Fact]
    public void TryDismantle_ThirteenBlocks_CreditsTenFloorsAndOnePlanExactlyOnce()
    {
        MatchRecord match = Create(
            new[] { 3, 5, 7, 9, 11, 13, 41, 43, 45, 47 },
            new[] { 2, 4, 6, 8, 10, 12, 14, 40, 42, 81 },
            [1, 70, 71]);
        Guid opener = match.ToBoard().CurrentTurnPlayerId;
        for (int module = 3; module <= 14; module++)
        {
            Place(match, module);
        }

        MatchBoardSnapshot before = match.ToBoard();
        MatchPrivateHand hand = match.ToPrivateHand(opener)!;
        Assert.Equal(opener, before.CurrentTurnPlayerId);
        Assert.Equal(13, before.Piles[0].CardCount);
        Assert.Equal(0, before.Players.Single(player => player.PlayerId == opener).FloorsBuilt);
        MatchDismantle move = new(1, before.Revision);

        MatchDismantleResult result = match.TryDismantle(opener, move, Now);
        MatchDismantleResult repeated = match.TryDismantle(opener, move, Now);

        Assert.Equal(MatchDismantleResult.Applied, result);
        Assert.Equal(MatchDismantleResult.StaleRevision, repeated);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(1, after.RoundNumber);
        Assert.Equal(before.Players.Single(player => player.PlayerId != opener).PlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(70, after.Piles[0].TopCardNumber);
        Assert.Equal(1, after.Piles[0].CardCount);
        Assert.Equal(10, after.Players.Single(player => player.PlayerId == opener).FloorsBuilt);
        Assert.Equal(0, after.Players.Single(player => player.PlayerId == opener).GlobalFloors);
        Assert.Equal(1, match.ToPrivateHand(opener)!.MasterPlans);
        Assert.Equal(hand.Cards, match.ToPrivateHand(opener)!.Cards);
    }

    [Fact]
    public void TryDismantle_EmptyDeckAndAnotherPlayerCanPlace_LeavesSpaceEmptyAndContinues()
    {
        MatchRecord match = CreateMatchWithTwoTowersAndEmptyDeck();

        MatchBoardSnapshot before = match.ToBoard();
        Guid blocked = before.CurrentTurnPlayerId;
        MatchDismantleResult result = match.TryDismantle(blocked, new MatchDismantle(2, before.Revision), Now);

        Assert.Equal(MatchDismantleResult.Applied, result);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(1, after.RoundNumber);
        Assert.Equal(match.Session.PlayerIds[0], after.CurrentTurnPlayerId);
        Assert.Null(after.Piles[1].TopCardNumber);
        Assert.Equal(0, after.Piles[1].CardCount);
        Assert.Equal(1, after.Players.Single(player => player.PlayerId == blocked).FloorsBuilt);
        Assert.Equal(0, match.ToPrivateHand(blocked)!.MasterPlans);
    }

    [Theory]
    [InlineData(-1L, MatchDismantleResult.StaleRevision)]
    [InlineData(0L, MatchDismantleResult.NotYourTurn)]
    public void TryDismantle_WaitingPlayerRequest_PreservesBoardAndHands(long revisionOffset, MatchDismantleResult expected)
    {
        AssertRejectedDismantle(true, (2, revisionOffset), expected);
    }

    [Theory]
    [InlineData(2, MatchDismantleResult.EmptyPile)]
    [InlineData(5, MatchDismantleResult.InvalidPile)]
    public void TryDismantle_InvalidActivePlayerPile_PreservesBoardAndHands(int pilePosition, MatchDismantleResult expected)
    {
        AssertRejectedDismantle(false, (pilePosition, 0L), expected);
    }

    private static void AssertRejectedDismantle(
        bool useWaitingPlayer, (int PilePosition, long RevisionOffset) request, MatchDismantleResult expected)
    {
        MatchRecord match = CreateMatchWithTwoTowersAndEmptyDeck();
        Guid active = match.Session.PlayerIds[0];
        Guid waiting = match.Session.PlayerIds[1];
        Dismantle(match, 2);
        MatchBoardSnapshot before = match.ToBoard();
        MatchPrivateHand activeHand = match.ToPrivateHand(active)!;
        MatchPrivateHand waitingHand = match.ToPrivateHand(waiting)!;

        Guid playerId = useWaitingPlayer ? waiting : active;
        MatchDismantle move = new(request.PilePosition, before.Revision + request.RevisionOffset);

        MatchDismantleResult result = match.TryDismantle(playerId, move, Now);

        Assert.Equal(expected, result);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(before.Revision, after.Revision);
        Assert.Equal(before.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(before.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)),
            after.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)));
        Assert.Equal(before.Players.Select(player => (player.FloorsBuilt, player.GlobalFloors)),
            after.Players.Select(player => (player.FloorsBuilt, player.GlobalFloors)));
        Assert.Equal(activeHand.Cards, match.ToPrivateHand(active)!.Cards);
        Assert.Equal(waitingHand.Cards, match.ToPrivateHand(waiting)!.Cards);
        Assert.Equal(activeHand.MasterPlans, match.ToPrivateHand(active)!.MasterPlans);
        Assert.Equal(waitingHand.MasterPlans, match.ToPrivateHand(waiting)!.MasterPlans);
    }

    [Fact]
    public void TryDismantle_ThreeBlocksAndExhaustedDeck_CreditsFloorsAndResetsRoundState()
    {
        MatchRecord match = Create(
            new[] { 31, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            new[] { 32, 81, 10, 11, 12, 13, 14, 15, 16, 17 },
            [30, 80]);
        Guid scorer = match.Session.PlayerIds[0];
        Guid other = match.Session.PlayerIds[1];
        Place(match, 31);
        Place(match, 32);
        MatchBoardSnapshot before = match.ToBoard();

        MatchDismantleResult result = match.TryDismantle(scorer, new MatchDismantle(1, before.Revision), Now);
        if (match.ToBoard().RoundNumber == 1)
        {
            Assert.Equal(1, match.ToPrivateHand(scorer)!.MasterPlans);
            Place(match, 81);
        }

        Assert.Equal(MatchDismantleResult.Applied, result);
        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(2, after.RoundNumber);
        Assert.Equal(other, after.CurrentTurnPlayerId);
        Assert.Equal(13, after.Players.Single(player => player.PlayerId == scorer).GlobalFloors);
        Assert.All(after.Players, player => Assert.Equal(0, player.FloorsBuilt));
        Assert.All(match.Session.PlayerIds, playerId =>
        {
            MatchPrivateHand hand = match.ToPrivateHand(playerId)!;
            Assert.Equal(0, hand.MasterPlans);
            Assert.Equal(10, hand.Cards.Count);
        });
    }

    [Fact]
    public void TryDismantle_ExhaustionAcrossThreeRounds_ScoresAndFinishesWithWinner()
    {
        MatchRecord match = Create(Enumerable.Range(1, 10), Enumerable.Range(11, 10), [90]);
        Guid initialOpener = match.ToBoard().CurrentTurnPlayerId;
        Guid other = match.Session.PlayerIds.Single(playerId => playerId != initialOpener);

        for (int round = 1; round <= 3; round++)
        {
            MatchBoardSnapshot board = match.ToBoard();
            Assert.Equal(round, board.RoundNumber);
            Guid opener = round == 2 ? other : initialOpener;
            Assert.Equal(opener, board.CurrentTurnPlayerId);
            Assert.Equal(MatchDismantleResult.Applied,
                match.TryDismantle(opener, new MatchDismantle(1, board.Revision), Now));
            Assert.Equal(board.Revision + 1, match.ToBoard().Revision);
            if (round < 3)
            {
                Assert.All(match.ToBoard().Players, player => Assert.Equal(0, player.FloorsBuilt));
                Assert.Equal(1, Assert.Single(match.ToBoard().Piles).CardCount);
            }
        }

        MatchBoardSnapshot final = match.ToBoard();
        Assert.Equal(MatchLifecycleStatus.Finished, match.Lifecycle.Status);
        Assert.Equal(Guid.Empty, final.CurrentTurnPlayerId);
        Assert.Equal(initialOpener, Assert.Single(final.WinnerIds));
        Assert.Equal(22, final.Players.Single(player => player.PlayerId == initialOpener).GlobalFloors);
        Assert.Equal(11, final.Players.Single(player => player.PlayerId == other).GlobalFloors);
        Assert.Equal(1, final.Players.Single(player => player.PlayerId == initialOpener).FinalPosition);
        Assert.Equal(2, final.Players.Single(player => player.PlayerId == other).FinalPosition);
        Assert.Equal(MatchDismantleResult.MatchUnavailable,
            match.TryDismantle(initialOpener, new MatchDismantle(1, final.Revision), Now));
    }

    [Fact]
    public void TryPlace_EmptyHand_ClosesRoundWithoutAwardingFloors()
    {
        MatchRecord match = Create(
            Enumerable.Range(1, 10).Select(module => module * 2),
            Enumerable.Range(1, 10).Select(module => module * 2 + 1),
            [1, 90, 95, 100, 70]);
        Guid opener = match.ToBoard().CurrentTurnPlayerId;
        int firstModule = match.ToPrivateHand(opener)!.Cards.Min();
        for (int module = firstModule; module < firstModule + 19; module++)
        {
            Place(match, module);
        }

        MatchBoardSnapshot next = match.ToBoard();
        Assert.Equal(2, next.RoundNumber);
        Assert.Equal(20, next.Revision);
        Assert.Equal(match.Session.PlayerIds.Single(playerId => playerId != opener), next.CurrentTurnPlayerId);
        Assert.All(next.Players, player => Assert.Equal(0, player.GlobalFloors));
        MatchRoundSummary closure = Assert.Single(next.RoundSummaries);
        Assert.Equal(1, closure.RoundNumber);
        Assert.Equal(opener, closure.ClosedByPlayerId);
        Assert.All(closure.Players, player => Assert.Equal(0, player.BonusFloors));
        Assert.All(match.Session.PlayerIds, playerId => Assert.Equal(10, match.ToPrivateHand(playerId)!.Cards.Count));
    }

    [Fact]
    public void TryDismantle_LastDeckCard_RefillsTheSameTowerWithoutGrowing()
    {
        MatchRecord match = Create(Enumerable.Range(1, 10), Enumerable.Range(81, 10), [70, 80]);
        MatchBoardSnapshot before = match.ToBoard();

        Dismantle(match, 1);

        MatchBoardSnapshot after = match.ToBoard();
        MatchBoardPile tower = Assert.Single(after.Piles);
        Assert.Equal(1, tower.Position);
        Assert.Equal(80, tower.TopCardNumber);
        Assert.Equal(1, tower.CardCount);
        Assert.Equal(1, after.RoundNumber);
        Assert.Equal(before.Revision + 1, after.Revision);
    }

    [Fact]
    public void TryDismantle_NextRound_ResetsAnExpandedBoardToOneTower()
    {
        MatchRecord match = CreateMatchWithTwoTowersAndEmptyDeck();
        Assert.Equal(2, match.ToBoard().Piles.Count);

        Dismantle(match, 2);
        Place(match, 2);
        Dismantle(match, 1);

        MatchBoardSnapshot next = match.ToBoard();
        Assert.Equal(2, next.RoundNumber);
        MatchBoardPile tower = Assert.Single(next.Piles);
        Assert.Equal(1, tower.Position);
        Assert.Equal(80, tower.TopCardNumber);
        Assert.Equal(1, tower.CardCount);
    }

    [Fact]
    public void TryDismantle_GrowsOneTowerPerActionThenCapsAtSix()
    {
        MatchRecord match = Create(Enumerable.Range(1, 10), Enumerable.Range(11, 10),
            Enumerable.Range(80, 14).ToArray());

        Assert.Equal(80, Assert.Single(match.ToBoard().Piles).TopCardNumber);
        for (int expected = 2; expected <= 6; expected++)
        {
            Dismantle(match, 1);
            MatchBoardSnapshot board = match.ToBoard();
            Assert.Equal(Enumerable.Range(1, expected), board.Piles.Select(pile => pile.Position));
            Assert.Equal(80 + 2 * (expected - 1), board.Piles[^1].TopCardNumber);
            Assert.All(board.Piles, pile => Assert.Equal(1, pile.CardCount));
        }

        Dismantle(match, 6);
        MatchBoardSnapshot capped = match.ToBoard();
        Assert.Equal(6, capped.Piles.Count);
        Assert.Equal(91, capped.Piles[^1].TopCardNumber);
        Assert.Equal(1, capped.RoundNumber);
    }

    [Fact]
    public void TryDismantle_ReusesAnEmptySpaceWhenReturnedCardsRefillTheDeck()
    {
        Guid[] players = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        MatchSessionSnapshot session = new()
        {
            MatchId = Guid.NewGuid(),
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            InitialHostPlayerId = players[0],
            PlayerIds = players,
            CreatedAtUtc = Now.UtcDateTime
        };
        int[] modules = Enumerable.Range(2, 10).Concat(Enumerable.Range(50, 10))
            .Concat(Enumerable.Range(61, 10)).Concat(new[] { 80, 1, 90 }).ToArray();
        MatchRecord match = new(session, Now, (() => new MatchDeck(modules), _ => 0));
        Dismantle(match, 1);
        Dismantle(match, 2);
        Assert.Null(match.ToBoard().Piles[1].TopCardNumber);

        Assert.True(match.TryLeaveMatch(players[2], Now).IsAccepted);
        Place(match, 2);
        Dismantle(match, 1);

        MatchBoardSnapshot after = match.ToBoard();
        Assert.Equal(new[] { 1, 2 }, after.Piles.Select(pile => pile.Position));
        Assert.Equal(new int?[] { 61, 62 }, after.Piles.Select(pile => pile.TopCardNumber));
    }

    private static MatchRecord CreateMatchWithTwoTowersAndEmptyDeck()
    {
        MatchRecord match = Create(
            new[] { 2, 4, 40, 41, 42, 43, 44, 45, 46, 47 }, Enumerable.Range(50, 10),
            [80, 1, 90]);
        Dismantle(match, 1);
        return match;
    }

    private static void Dismantle(MatchRecord match, int position)
    {
        MatchBoardSnapshot board = match.ToBoard();
        MatchDismantle move = new(position, board.Revision);
        MatchDismantleResult result = match.TryDismantle(board.CurrentTurnPlayerId, move, Now);
        Assert.Equal(MatchDismantleResult.Applied, result);
    }

    private static void Place(MatchRecord match, int module, int pilePosition = 1)
    {
        MatchBoardSnapshot board = match.ToBoard();
        MatchPlacement action = new(module, pilePosition, board.Revision);

        Assert.Equal(MatchPlacementResult.Applied, match.TryPlace(board.CurrentTurnPlayerId, action, Now));
    }

    private static MatchRecord Create(
        IEnumerable<int> firstHand, IEnumerable<int> secondHand, int[] towerAndDeckCards)
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
        int[] modules = firstHand.Concat(secondHand).Concat(towerAndDeckCards).ToArray();
        Assert.Equal(modules.Length, modules.Distinct().Count());

        return new MatchRecord(session, Now, (() => new MatchDeck(modules), _ => 0));
    }
}
