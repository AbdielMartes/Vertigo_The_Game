using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchPrivateHandTests
{
    private static (InMemoryMatchSessionStarter Game, Guid MatchId, Guid Host, Guid Guest) StartMatch()
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
        return (game, game.Start(request).MatchId, host, guest);
    }

    [Fact]
    public void GetPrivateHand_OwnPlayer_ReturnsSixDistinctModules()
    {
        var state = StartMatch();

        var hand = state.Game.GetPrivateHand(state.MatchId, state.Host);

        Assert.NotNull(hand);
        Assert.Equal(10, hand.Cards.Count);
        Assert.Equal(10, hand.Cards.Distinct().Count());
        Assert.All(hand.Cards, card => Assert.InRange(card, 1, 100));
    }

    [Fact]
    public void GetPrivateHand_AnotherMatchNonmember_ReturnsNull()
    {
        var state = StartMatch();

        var hand = state.Game.GetPrivateHand(state.MatchId, Guid.NewGuid());

        Assert.Null(hand);
    }

    [Fact]
    public void GetPrivateHand_TwoPlayers_DoNotShareModules()
    {
        var state = StartMatch();

        var first = state.Game.GetPrivateHand(state.MatchId, state.Host);
        var second = state.Game.GetPrivateHand(state.MatchId, state.Guest);

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.Empty(first.Cards.Intersect(second.Cards));
    }

    [Fact]
    public void GetPrivateHand_MutatedResponse_DoesNotChangeServerHand()
    {
        var state = StartMatch();
        var first = state.Game.GetPrivateHand(state.MatchId, state.Host);
        Assert.NotNull(first);
        int original = first.Cards[0];
        first.Cards[0] = -1;
        first.Cards.Clear();

        var again = state.Game.GetPrivateHand(state.MatchId, state.Host);

        Assert.NotNull(again);
        Assert.Equal(10, again.Cards.Count);
        Assert.Equal(original, again.Cards[0]);
    }

    [Fact]
    public void Start_SameLobby_IsIdempotentAndKeepsHand()
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
        var before = game.GetPrivateHand(matchId, host);
        Guid repeated = game.Start(request).MatchId;

        var after = game.GetPrivateHand(repeated, host);

        Assert.Equal(matchId, repeated);
        Assert.NotNull(before);
        Assert.NotNull(after);
        Assert.Equal(before.Cards, after.Cards);
    }

    [Fact]
    public void GetBoard_AfterDeal_DoesNotExposePrivateCards()
    {
        var state = StartMatch();

        var board = state.Game.GetBoard(state.MatchId);

        Assert.NotNull(board);
        Assert.Equal(4, board.Piles.Count);
        Assert.All(board.Piles, pile => Assert.InRange(pile.TopCardNumber!.Value, 1, 100));
    }

    [Fact]
    public void GetPrivateHand_AfterReconnect_KeepsSameHand()
    {
        var state = StartMatch();
        var before = state.Game.GetPrivateHand(state.MatchId, state.Guest);
        state.Game.TryDisconnect(state.MatchId, state.Guest);
        state.Game.TryReconnect(state.MatchId, state.Guest);

        var after = state.Game.GetPrivateHand(state.MatchId, state.Guest);

        Assert.NotNull(before);
        Assert.NotNull(after);
        Assert.Equal(before.Cards, after.Cards);
    }
}
