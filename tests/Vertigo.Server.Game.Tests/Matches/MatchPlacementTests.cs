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
        Assert.Equal(4, foundations.Length);
        Assert.Equal(10, host.Cards.Count);
        Assert.Equal(10, other.Cards.Count);
        Assert.Equal(24, foundations.Concat(host.Cards).Concat(other.Cards).Distinct().Count());
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
        Assert.Equal(board.Revision, state.Game.GetBoard(state.MatchId)!.Revision);
    }

    [Fact]
    public void TryPlace_ForgedCard_DoesNotMutateBoard()
    {
        var state = Create();
        var board = state.Game.GetBoard(state.MatchId)!;
        Guid waitingPlayer = board.Players.Single(player => player.PlayerId != board.CurrentTurnPlayerId).PlayerId;
        MatchPrivateHand other = state.Game.GetPrivateHand(state.MatchId, waitingPlayer)!;
        var move = new MatchPlacement(other.Cards[0], 1, board.Revision);
        Assert.Equal(MatchPlacementResult.CardNotOwned,
            state.Game.TryPlace(state.MatchId, board.CurrentTurnPlayerId, move));
        Assert.Equal(board.Revision, state.Game.GetBoard(state.MatchId)!.Revision);
    }

    [Fact]
    public void TryPlace_ValidCard_ConsumesItAndAdvancesTurn()
    {
        // Repeatedly create a randomized initial board until a valid move is available.
        // Avoid probabilistic test failure: check only within a bounded number of attempts.
        for (int attempt = 0; attempt < 100; attempt++)
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
            Assert.DoesNotContain(match.card, state.Game.GetPrivateHand(state.MatchId, openingPlayer)!.Cards);
            Assert.Equal(MatchPlacementResult.StaleRevision,
                state.Game.TryPlace(state.MatchId, openingPlayer, move));
            return;
        }
        throw new InvalidOperationException("No playable test board generated in 100 attempts.");
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
    }
}
