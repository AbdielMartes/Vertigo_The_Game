using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchInitialBoardTests
{
    [Fact]
    public void GetBoard_AfterStart_InitializesRoundAndHostTurn()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid matchId = game.Start(request).MatchId;
        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        Assert.Equal(1, board.Revision);
        Assert.Equal(1, board.RoundNumber);
        Assert.Equal(request.HostPlayerId, board.CurrentTurnPlayerId);
        Assert.Equal(request.PlayerIds, board.Players.Select(player => player.PlayerId));
        Assert.All(board.Players, player => Assert.Equal(0, player.FloorsBuilt));
    }

    [Fact]
    public void GetBoard_AfterStart_HasFourOrderedFoundations()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid matchId = game.Start(CreateRequest()).MatchId;

        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        Assert.Equal(new[] { 1, 2, 3, 4 }, board.Piles.Select(pile => pile.Position));
        Assert.All(board.Piles, pile =>
        {
            Assert.Equal(1, pile.CardCount);
            Assert.InRange(pile.TopCardNumber!.Value, 1, 100);
        });
    }

    [Fact]
    public void Start_SameLobbyRequest_ReusesMatchAndInitialBoard()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid firstId = game.Start(request).MatchId;
        Guid secondId = game.Start(request).MatchId;
        MatchBoardSnapshot? board = game.GetBoard(secondId);

        Assert.Equal(firstId, secondId);
        Assert.NotNull(board);
        Assert.Equal(1, board.Revision);
        Assert.Equal(1, board.RoundNumber);
    }

    [Fact]
    public void GetBoard_AfterMutatingSnapshot_KeepsOriginalState()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid matchId = game.Start(CreateRequest()).MatchId;
        MatchBoardSnapshot? first = game.GetBoard(matchId);
        Assert.NotNull(first);
        int? foundation = first.Piles[0].TopCardNumber;
        first.Piles[0].Position = 99;
        first.Piles[0].TopCardNumber = 50;
        first.Piles[0].CardCount = 10;
        first.Piles.Clear();
        first.RoundNumber = 3;
        first.CurrentTurnPlayerId = Guid.NewGuid();

        MatchBoardSnapshot? second = game.GetBoard(matchId);

        Assert.NotNull(second);
        Assert.Equal(1, second.RoundNumber);
        Assert.Equal(4, second.Piles.Count);
        Assert.Equal(1, second.Piles[0].Position);
        Assert.Equal(foundation, second.Piles[0].TopCardNumber);
        Assert.Equal(1, second.Piles[0].CardCount);
    }

    [Fact]
    public void GetBoard_DifferentMatches_KeepsIndependentInitialState()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest firstRequest = CreateRequest();
        MatchStartRequest secondRequest = CreateRequest();

        Guid firstId = game.Start(firstRequest).MatchId;
        Guid secondId = game.Start(secondRequest).MatchId;
        MatchBoardSnapshot? first = game.GetBoard(firstId);
        MatchBoardSnapshot? second = game.GetBoard(secondId);

        Assert.NotEqual(firstId, secondId);
        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Equal(firstRequest.HostPlayerId, first.CurrentTurnPlayerId);
        Assert.Equal(secondRequest.HostPlayerId, second.CurrentTurnPlayerId);
        Assert.Equal(4, first.Piles.Count);
        Assert.Equal(4, second.Piles.Count);
    }

    [Fact]
    public void GetBoard_AfterDisconnectAndReconnect_PreservesTurnAndRound()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        Guid matchId = game.Start(request).MatchId;

        Assert.True(game.TryDisconnect(matchId, request.HostPlayerId));
        Assert.Equal(MatchReconnectResult.Reconnected, game.TryReconnect(matchId, request.HostPlayerId));
        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        Assert.Equal(1, board.RoundNumber);
        Assert.Equal(request.HostPlayerId, board.CurrentTurnPlayerId);
        Assert.Equal(1, board.Revision);
    }

    private static MatchStartRequest CreateRequest()
    {
        Guid host = Guid.NewGuid();
        return new MatchStartRequest
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };
    }
}
