using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchInitialBoardTests
{
    [Fact]
    public void GetBoard_AfterStart_InitializesRoundAndSelectsParticipant()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid matchId = game.Start(request).MatchId;
        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        Assert.Equal(1, board.Revision);
        Assert.Equal(1, board.RoundNumber);
        Assert.Contains(board.CurrentTurnPlayerId, request.PlayerIds);
        Assert.Equal(request.PlayerIds, board.Players.Select(player => player.PlayerId));
        Assert.All(board.Players, player => Assert.Equal(0, player.FloorsBuilt));
    }

    [Fact]
    public void GetBoard_AfterStart_HasOneFoundation()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid matchId = game.Start(CreateRequest()).MatchId;

        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        MatchBoardPile pile = Assert.Single(board.Piles);
        Assert.Equal(1, pile.Position);
        Assert.Equal(1, pile.CardCount);
        Assert.InRange(pile.TopCardNumber!.Value, 1, 100);
    }

    [Fact]
    public void Start_SixParticipants_DealsTenUniqueModulesPerPlayerAndOneFoundation()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid[] players = Enumerable.Range(0, 6).Select(index => Guid.NewGuid()).ToArray();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = players[0],
            PlayerIds = players
        };

        Guid matchId = game.Start(request).MatchId;

        MatchBoardSnapshot? board = game.GetBoard(matchId);
        Assert.NotNull(board);
        List<int> dealt = board.Piles.Select(pile => pile.TopCardNumber!.Value).ToList();
        Assert.Single(dealt);
        foreach (Guid player in players)
        {
            MatchPrivateHand? hand = game.GetPrivateHand(matchId, player);
            Assert.NotNull(hand);
            Assert.Equal(10, hand.Cards.Count);
            dealt.AddRange(hand.Cards);
        }

        Assert.Equal(61, dealt.Distinct().Count());
        Assert.All(dealt, module => Assert.InRange(module, 1, 100));
    }

    [Fact]
    public void Start_SameLobbyRequest_ReusesMatchAndInitialBoard()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();

        Guid firstId = game.Start(request).MatchId;
        MatchBoardSnapshot? initial = game.GetBoard(firstId);
        Guid secondId = game.Start(request).MatchId;
        MatchBoardSnapshot? board = game.GetBoard(secondId);

        Assert.Equal(firstId, secondId);
        Assert.NotNull(initial);
        Assert.NotNull(board);
        Assert.Equal(initial.CurrentTurnPlayerId, board.CurrentTurnPlayerId);
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
        Assert.Single(second.Piles);
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
        Assert.Contains(first.CurrentTurnPlayerId, firstRequest.PlayerIds);
        Assert.Contains(second.CurrentTurnPlayerId, secondRequest.PlayerIds);
        Assert.Single(first.Piles);
        Assert.Single(second.Piles);
    }

    [Fact]
    public void GetBoard_AfterDisconnectAndReconnect_PreservesTurnAndRound()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        MatchStartRequest request = CreateRequest();
        Guid matchId = game.Start(request).MatchId;

        MatchBoardSnapshot? initial = game.GetBoard(matchId);
        Assert.NotNull(initial);
        Guid openingPlayer = initial.CurrentTurnPlayerId;
        Assert.True(game.TryDisconnect(matchId, openingPlayer));
        Assert.Equal(MatchReconnectResult.Reconnected, game.TryReconnect(matchId, openingPlayer));
        MatchBoardSnapshot? board = game.GetBoard(matchId);

        Assert.NotNull(board);
        Assert.Equal(1, board.RoundNumber);
        Assert.Equal(openingPlayer, board.CurrentTurnPlayerId);
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
