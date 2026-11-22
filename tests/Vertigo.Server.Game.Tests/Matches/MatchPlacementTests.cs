using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchPlacementTests
{
    private static (InMemoryMatchSessionStarter Game, Guid MatchId, Guid Host, Guid Other) Create()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = host,
            PlayerIds = new[] { host, other }
        };
        return (game, game.Start(request).MatchId, host, other);
    }

    [Fact]
    public void Start_FoundationsAndHands_UseDistinctModules()
    {
        var state = Create();
        var board = state.Game.GetBoard(state.MatchId)!;
        var host = state.Game.GetPrivateHand(state.MatchId, state.Host)!;
        var other = state.Game.GetPrivateHand(state.MatchId, state.Other)!;
        int[] foundations = board.Piles.Select(pile => pile.TopCardNumber!.Value).ToArray();
        Assert.Single(foundations);
        Assert.Equal(10, host.Cards.Count);
        Assert.Equal(10, other.Cards.Count);
        Assert.Equal(21, foundations.Concat(host.Cards).Concat(other.Cards).Distinct().Count());
        Assert.All(board.Piles, pile => Assert.Equal(1, pile.CardCount));
    }

    [Fact]
    public void TryPlace_OtherPlayerTurn_DoesNotMutateBoard()
    {
        var state = Create();
        var board = state.Game.GetBoard(state.MatchId)!;
        Guid waitingPlayer = board.Players.Single(player => player.PlayerId != board.CurrentTurnPlayerId).PlayerId;
        MatchPrivateHand hand = state.Game.GetPrivateHand(state.MatchId, waitingPlayer)!;
        var move = new MatchPlacement(hand.Cards[0], 1, board.Revision);
        Assert.Equal(MatchPlacementResult.NotYourTurn,
            state.Game.TryPlace(state.MatchId, waitingPlayer, move));
        AssertPlacementStateUnchanged(state.Game, board, hand);
    }

    [Fact]
    public void TryPlace_ForgedCard_DoesNotMutateBoard()
    {
        var state = Create();
        var board = state.Game.GetBoard(state.MatchId)!;
        Guid waitingPlayer = board.Players.Single(player => player.PlayerId != board.CurrentTurnPlayerId).PlayerId;
        MatchPrivateHand other = state.Game.GetPrivateHand(state.MatchId, waitingPlayer)!;
        MatchPrivateHand activeHand = state.Game.GetPrivateHand(state.MatchId, board.CurrentTurnPlayerId)!;
        var move = new MatchPlacement(other.Cards[0], 1, board.Revision);
        Assert.Equal(MatchPlacementResult.CardNotOwned,
            state.Game.TryPlace(state.MatchId, board.CurrentTurnPlayerId, move));
        AssertPlacementStateUnchanged(state.Game, board, activeHand);
    }

    [Theory]
    [InlineData(MatchLifecycleStatus.Suspended)]
    [InlineData(MatchLifecycleStatus.Finished)]
    [InlineData(MatchLifecycleStatus.Cancelled)]
    public void TryPlace_InactiveMatchWithStaleRevision_ReturnsUnavailableWithoutMutatingState(
        MatchLifecycleStatus status)
    {
        var (game, matchId, host, other) = Create();
        bool changed = status switch
        {
            MatchLifecycleStatus.Suspended => game.TryDisconnect(matchId, host) && game.TryDisconnect(matchId, other),
            MatchLifecycleStatus.Finished => game.TryFinish(matchId),
            MatchLifecycleStatus.Cancelled => game.TryCancel(matchId),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
        Assert.True(changed);
        Assert.Equal(status, game.Get(matchId)!.Status);

        MatchBoardSnapshot board = game.GetBoard(matchId)!;
        Guid playerId = board.CurrentTurnPlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(matchId, playerId)!;
        MatchPlacement move = new(hand.Cards[0], 1, board.Revision - 1);

        MatchPlacementResult result = game.TryPlace(matchId, playerId, move);

        Assert.Equal(MatchPlacementResult.MatchUnavailable, result);
        AssertPlacementStateUnchanged(game, board, hand);
        Assert.Equal(status, game.Get(matchId)!.Status);
    }

    [Fact]
    public void TryPlace_StaleRevisionOutsideTurn_ReturnsStaleRevisionWithoutMutatingState()
    {
        var (game, matchId, _, _) = Create();
        MatchBoardSnapshot board = game.GetBoard(matchId)!;
        Guid waitingPlayer = board.Players.Single(player => player.PlayerId != board.CurrentTurnPlayerId).PlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(matchId, waitingPlayer)!;
        MatchPlacement move = new(hand.Cards[0], 1, board.Revision - 1);

        MatchPlacementResult result = game.TryPlace(matchId, waitingPlayer, move);

        Assert.Equal(MatchPlacementResult.StaleRevision, result);
        AssertPlacementStateUnchanged(game, board, hand);
    }

    [Fact]
    public void TryPlace_UnstablePlacement_RejectsWithoutMutatingState()
    {
        var (game, matchId, _, _) = Create();
        MatchBoardSnapshot board = game.GetBoard(matchId)!;
        Guid playerId = board.CurrentTurnPlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(matchId, playerId)!;
        MatchPlacement invalidPlacement = (from pile in board.Piles
                                from card in hand.Cards
                                where card - pile.TopCardNumber!.Value is < 1 or > 10
                                select new MatchPlacement(card, pile.Position, board.Revision)).First();

        MatchPlacementResult result = game.TryPlace(matchId, playerId, invalidPlacement);

        Assert.Equal(MatchPlacementResult.InvalidPlacement, result);
        AssertPlacementStateUnchanged(game, board, hand);
    }

    [Fact]
    public void TryPlace_ValidCard_ConsumesItAndAdvancesTurn()
    {
        const int MaximumAttempts = 100;
        bool verified = false;
        for (int attempt = 0; attempt < MaximumAttempts && !verified; attempt++)
        {
            var state = Create();
            var board = state.Game.GetBoard(state.MatchId)!;
            Guid openingPlayer = board.CurrentTurnPlayerId;
            Guid waitingPlayer = board.Players.Single(player => player.PlayerId != openingPlayer).PlayerId;
            MatchPrivateHand hand = state.Game.GetPrivateHand(state.MatchId, openingPlayer)!;
            var match = (from card in hand.Cards
                         from pile in board.Piles
                         where MatchPlacementRules.IsValid(pile.TopCardNumber!.Value, card)
                         select (card, pile.Position)).FirstOrDefault();
            if (match.card == 0)
            {
                continue;
            }
            var move = new MatchPlacement(match.card, match.Position, board.Revision);
            Assert.Equal(MatchPlacementResult.Applied,
                state.Game.TryPlace(state.MatchId, openingPlayer, move));
            var next = state.Game.GetBoard(state.MatchId)!;
            Assert.Equal(board.Revision + 1, next.Revision);
            Assert.Equal(waitingPlayer, next.CurrentTurnPlayerId);
            Assert.Equal(match.card, next.Piles[match.Position - 1].TopCardNumber);
            Assert.Equal(board.Piles[match.Position - 1].CardCount + 1, next.Piles[match.Position - 1].CardCount);
            MatchPrivateHand afterMove = state.Game.GetPrivateHand(state.MatchId, openingPlayer)!;
            Assert.Equal(hand.Cards.Where(card => card != match.card), afterMove.Cards);
            Assert.Equal(MatchPlacementResult.StaleRevision,
                state.Game.TryPlace(state.MatchId, openingPlayer, move));
            AssertPlacementStateUnchanged(state.Game, next, afterMove);
            verified = true;
        }

        Assert.True(verified, "No playable test board was generated.");
    }

    [Fact]
    public void TryPlace_UnknownPile_IsRejected()
    {
        var state = Create();
        var board = state.Game.GetBoard(state.MatchId)!;
        Guid openingPlayer = board.CurrentTurnPlayerId;
        MatchPrivateHand hand = state.Game.GetPrivateHand(state.MatchId, openingPlayer)!;
        Assert.Equal(MatchPlacementResult.InvalidPile,
            state.Game.TryPlace(state.MatchId, openingPlayer, new MatchPlacement(hand.Cards[0], 5, board.Revision)));
        AssertPlacementStateUnchanged(state.Game, board, hand);
    }

    private static void AssertPlacementStateUnchanged(
        InMemoryMatchSessionStarter game, MatchBoardSnapshot expectedBoard, MatchPrivateHand expectedHand)
    {
        MatchBoardSnapshot board = game.GetBoard(expectedBoard.MatchId)!;
        MatchPrivateHand hand = game.GetPrivateHand(expectedBoard.MatchId, expectedHand.PlayerId)!;

        Assert.Equal(expectedBoard.Revision, board.Revision);
        Assert.Equal(expectedBoard.CurrentTurnPlayerId, board.CurrentTurnPlayerId);
        Assert.Equal(expectedBoard.RoundNumber, board.RoundNumber);
        Assert.Equal(
            expectedBoard.Piles.Select(pile => (pile.Position, pile.TopCardNumber, pile.CardCount)),
            board.Piles.Select(pile => (pile.Position, pile.TopCardNumber, pile.CardCount)));
        Assert.Equal(expectedHand.Cards, hand.Cards);
    }
}
