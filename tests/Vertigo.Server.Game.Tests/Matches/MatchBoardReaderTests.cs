using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchBoardReaderTests
{
    [Fact]
    public void GetBoard_AfterStart_ContainsParticipantsAndFoundations()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid host = Guid.NewGuid();
        Guid guest = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = host,
            PlayerIds = new[] { host, guest }
        };

        MatchSessionSnapshot match = game.Start(request);
        var board = game.GetBoard(match.MatchId);

        Assert.NotNull(board);
        Assert.Equal(match.MatchId, board.MatchId);
        Assert.Equal(1, board.Revision);
        Assert.Equal(new[] { host, guest }, board.Players.Select(p => p.PlayerId));
        Assert.All(board.Players, p => Assert.Equal(0, p.FloorsBuilt));
        Assert.Single(board.Piles);
        Assert.All(board.Piles, pile => Assert.Equal(1, pile.CardCount));
        Assert.All(board.Piles, pile => Assert.InRange(pile.TopCardNumber!.Value, 1, 100));
        Assert.Equal(1, board.RoundNumber);
        Assert.Contains(board.CurrentTurnPlayerId, request.PlayerIds);
    }

    [Fact]
    public void GetBoard_UnknownMatch_ReturnsNull()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);

        var board = game.GetBoard(Guid.NewGuid());

        Assert.Null(board);
    }

    [Fact]
    public void GetBoard_ChangedCopy_DoesNotMutateStoredBoard()
    {
        InMemoryMatchSessionStarter game = new(TimeProvider.System);
        Guid host = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };
        Guid matchId = game.Start(request).MatchId;
        var first = game.GetBoard(matchId);
        Assert.NotNull(first);
        first.Players[0].FloorsBuilt = 999;
        first.Players.Clear();

        var second = game.GetBoard(matchId);

        Assert.NotNull(second);
        Assert.Equal(2, second.Players.Count);
        Assert.Equal(0, second.Players[0].FloorsBuilt);
    }
}
