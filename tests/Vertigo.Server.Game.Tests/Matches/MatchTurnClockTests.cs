using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchTurnClockTests
{
    private static readonly DateTimeOffset StartTime = DateTimeOffset.UnixEpoch;

    [Fact]
    public void TryExpireTurn_AtDeadline_ExpelsOnlyOnceAndStartsNextTurn()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid active = before.CurrentTurnPlayerId;
        Guid[] order = before.Players.Select(player => player.PlayerId).ToArray();
        Guid nextPlayer = order[(Array.IndexOf(order, active) + 1) % order.Length];

        Assert.Equal(StartTime.AddSeconds(20), before.TurnDeadlineUtc);
        Assert.Equal(StartTime, before.ServerTimeUtc);
        clock.Set(StartTime.AddSeconds(20).AddTicks(-1));
        Assert.Null(game.TryExpireTurn(matchId));
        Assert.Equal(before.Revision, game.GetBoard(matchId)!.Revision);

        clock.Set(StartTime.AddSeconds(20));
        Assert.Equal(active, game.TryExpireTurn(matchId));
        Assert.Null(game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(nextPlayer, after.CurrentTurnPlayerId);
        Assert.Equal(StartTime.AddSeconds(40), after.TurnDeadlineUtc);
        Assert.Equal(StartTime.AddSeconds(20), after.ServerTimeUtc);
        Assert.Equal(MatchEndReason.None, after.EndReason);
        Assert.Equal(active, Assert.Single(after.ExpelledPlayerIds));
        Assert.DoesNotContain(after.Players, player => player.PlayerId == active);
        Assert.Null(game.GetPrivateHand(matchId, active));
        Assert.Equal(MatchReconnectResult.Rejected, game.TryReconnect(matchId, active));
        MatchBoardMove move = Assert.Single(after.RecentMoves);
        Assert.Equal(active, move.PlayerId);
        Assert.Equal(MatchBoardActionKind.TurnExpired, move.ActionKind);
        Assert.Equal(after.Revision, move.Revision);
    }

    [Fact]
    public void TryReconnect_BeforeDeadline_KeepsOriginalTurnAndDeadline()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 2);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid active = before.CurrentTurnPlayerId;

        clock.Set(StartTime.AddSeconds(8));
        Assert.True(game.TryDisconnect(matchId, active));
        clock.Set(StartTime.AddSeconds(12));
        Assert.Equal(MatchReconnectResult.Reconnected, game.TryReconnect(matchId, active));
        Assert.Equal(MatchPlacementResult.StaleRevision,
            game.TryPlace(matchId, active, new MatchPlacement(1, 1, before.Revision - 1)));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(before.TurnDeadlineUtc, after.TurnDeadlineUtc);
        Assert.Equal(before.Revision, after.Revision);
    }

    [Fact]
    public void TryExpireTurn_OnlyOnePlayerRemains_FinishesByAbandonment()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 2);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid winner = before.Players.Single(player => player.PlayerId != before.CurrentTurnPlayerId).PlayerId;

        clock.Set(StartTime.AddSeconds(20));
        Assert.Equal(before.CurrentTurnPlayerId, game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(MatchLifecycleStatus.Finished, game.Get(matchId)!.Status);
        Assert.Equal(MatchEndReason.Abandonment, after.EndReason);
        Assert.Equal(winner, Assert.Single(after.WinnerIds));
        Assert.Equal(Guid.Empty, after.CurrentTurnPlayerId);
        Assert.Null(after.TurnDeadlineUtc);
        Assert.Empty(after.RoundSummaries);
        Assert.Equal(0, Assert.Single(after.Players).GlobalFloors);
    }

    [Fact]
    public void TryExpireTurn_AllPlayersDisconnected_UsesOriginalDeadline()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 2);
        MatchBoardSnapshot board = game.GetBoard(matchId)!;
        clock.Set(StartTime.AddSeconds(12));
        Assert.All(board.Players, player => Assert.True(game.TryDisconnect(matchId, player.PlayerId)));
        Assert.Equal(MatchLifecycleStatus.Suspended, game.Get(matchId)!.Status);
        clock.Set(StartTime.AddSeconds(19));
        Assert.Null(game.TryExpireTurn(matchId));
        clock.Set(StartTime.AddSeconds(20));

        Guid? expelled = game.TryExpireTurn(matchId);

        Assert.Equal(board.CurrentTurnPlayerId, expelled);
        Assert.Equal(MatchEndReason.Abandonment, game.GetBoard(matchId)!.EndReason);
    }

    [Fact]
    public void TryExpireTurn_ConcurrentLegalActionAtDeadline_OnlyExpulsionTakesEffect()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Func<bool> playTurn = GetLegalTurn(game, before);
        bool played = false;
        Guid? expelled = null;
        clock.Set(StartTime.AddSeconds(20));

        Parallel.Invoke(() => played = playTurn(), () => expelled = game.TryExpireTurn(matchId));

        Assert.False(played);
        Assert.Equal(before.CurrentTurnPlayerId, expelled);
        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(MatchBoardActionKind.TurnExpired, Assert.Single(after.RecentMoves).ActionKind);
        Assert.Equal(before.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)),
            after.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)));
    }

    [Fact]
    public void TurnActions_AtDeadlineBeforeClockSweep_RejectWithoutChangingState()
    {
        ManualClock clock = new();
        (InMemoryMatchSessionStarter game, Guid matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid active = before.CurrentTurnPlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(matchId, active)!;
        clock.Set(before.TurnDeadlineUtc!.Value);

        MatchPlacementResult placement = game.TryPlace(
            matchId, active, new MatchPlacement(hand.Cards[0], 1, before.Revision));
        MatchPlacementResult plan = game.TryUseMasterPlan(
            matchId, active, new MatchMasterPlan(hand.Cards.Take(2).ToArray(), 1, before.Revision));
        MatchDismantleResult dismantle = game.TryDismantle(matchId, active, new MatchDismantle(1, before.Revision));

        Assert.Equal(MatchPlacementResult.TurnExpired, placement);
        Assert.Equal(MatchPlacementResult.TurnExpired, plan);
        Assert.Equal(MatchDismantleResult.TurnExpired, dismantle);
        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.Revision, after.Revision);
        Assert.Equal(before.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(before.TurnDeadlineUtc, after.TurnDeadlineUtc);
        Assert.Empty(after.RecentMoves);
        Assert.Equal(hand.Cards, game.GetPrivateHand(matchId, active)!.Cards);
    }

    [Fact]
    public void TryExpireTurn_LegalActionAcceptedBeforeDeadline_DoesNotExpireNewTurnEarly()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Func<bool> playTurn = GetLegalTurn(game, before);
        clock.Set(StartTime.AddSeconds(19));

        Assert.True(playTurn());
        clock.Set(StartTime.AddSeconds(20));
        Assert.Null(game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(StartTime.AddSeconds(39), after.TurnDeadlineUtc);
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Empty(after.ExpelledPlayerIds);
    }

    [Fact]
    public void TryExpireTurn_ExpelledHand_ReturnsAfterExistingModules()
    {
        Guid[] players = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        MatchSessionSnapshot session = new(
            Guid.NewGuid(), Guid.NewGuid(), "ABC234", players[0], players, StartTime.UtcDateTime);
        int[] modules = Enumerable.Range(1, 10).Concat(Enumerable.Range(21, 10))
            .Concat(Enumerable.Range(41, 10)).Concat(new[] { 90, 91, 92, 93, 95 }).ToArray();
        MatchRecord record = new(session, StartTime, () => new MatchDeck(modules));
        MatchBoardSnapshot board = record.ToBoard();
        int expectedReturnedModule = record.ToPrivateHand(board.CurrentTurnPlayerId)!.Cards[0];
        DateTimeOffset now = StartTime.AddSeconds(20);

        Assert.Equal(board.CurrentTurnPlayerId, record.TryExpireTurn(now));
        Assert.Equal(MatchEndReason.None, record.ToBoard().EndReason);
        Assert.Equal(1, record.ToBoard().RoundNumber);
        Guid nextPlayer = record.ToBoard().CurrentTurnPlayerId;
        Dismantle(record, now);
        Assert.Equal(95, record.ToBoard().Piles[0].TopCardNumber);
        Dismantle(record, now);
        Assert.Equal(expectedReturnedModule, record.ToBoard().Piles[0].TopCardNumber);
        Assert.Equal(nextPlayer, record.ToBoard().CurrentTurnPlayerId);
    }

    private static void Dismantle(MatchRecord record, DateTimeOffset now)
    {
        MatchBoardSnapshot board = record.ToBoard();
        Assert.Equal(MatchDismantleResult.Applied,
            record.TryDismantle(board.CurrentTurnPlayerId, new MatchDismantle(1, board.Revision), now));
    }

    private static Func<bool> GetLegalTurn(InMemoryMatchSessionStarter game, MatchBoardSnapshot board)
    {
        Guid active = board.CurrentTurnPlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(board.MatchId, active)!;
        (int Card, int Pile) placement = (from card in hand.Cards
                                         from pile in board.Piles
                                         where pile.TopCardNumber is int top && MatchPlacementRules.IsValid(top, card)
                                         select (card, pile.Position)).FirstOrDefault();

        return placement.Pile == 0
            ? () => game.TryDismantle(board.MatchId, active, new MatchDismantle(1, board.Revision))
                == MatchDismantleResult.Applied
            : () => game.TryPlace(board.MatchId, active, new MatchPlacement(placement.Card, placement.Pile,
                board.Revision)) == MatchPlacementResult.Applied;
    }

    private static (InMemoryMatchSessionStarter Game, Guid MatchId) StartMatch(ManualClock clock, int playerCount)
    {
        InMemoryMatchSessionStarter game = new(clock);
        Guid[] players = Enumerable.Range(0, playerCount).Select(_ => Guid.NewGuid()).ToArray();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = players[0],
            PlayerIds = players
        };

        return (game, game.Start(request).MatchId);
    }

    private sealed class ManualClock : TimeProvider
    {
        private DateTimeOffset _now = StartTime;

        public override DateTimeOffset GetUtcNow()
        {
            return _now;
        }

        public void Set(DateTimeOffset value)
        {
            _now = value;
        }
    }
}
