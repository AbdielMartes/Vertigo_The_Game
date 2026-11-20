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
    public void TryExpireTurn_AtDeadline_SkipsOnceAndStartsNextTurn()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid active = before.CurrentTurnPlayerId;
        Guid[] order = before.Players.Select(player => player.PlayerId).ToArray();
        Guid nextPlayer = order[(Array.IndexOf(order, active) + 1) % order.Length];

        Assert.Equal(StartTime.AddSeconds(30), before.TurnDeadlineUtc);
        Assert.Equal(StartTime, before.ServerTimeUtc);
        clock.Set(before.TurnDeadlineUtc!.Value.AddTicks(-1));
        Assert.Null(game.TryExpireTurn(matchId));
        Assert.Equal(before.Revision, game.GetBoard(matchId)!.Revision);

        clock.Set(before.TurnDeadlineUtc!.Value);
        Assert.Equal(active, game.TryExpireTurn(matchId));
        Assert.Null(game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(nextPlayer, after.CurrentTurnPlayerId);
        Assert.Equal(clock.GetUtcNow() + (before.TurnDeadlineUtc!.Value - StartTime), after.TurnDeadlineUtc);
        Assert.Equal(clock.GetUtcNow(), after.ServerTimeUtc);
        Assert.Equal(MatchEndReason.None, after.EndReason);
        Assert.Equal(clock.GetUtcNow().AddSeconds(30), after.PresenceDeadlinesUtc[active]);
        Assert.Empty(after.ExpelledPlayerIds);
        Assert.NotNull(game.GetPrivateHand(matchId, active));
        MatchBoardMove move = Assert.Single(after.RecentMoves);
        Assert.Equal(active, move.PlayerId);
        Assert.Equal(MatchBoardActionKind.TurnSkipped, move.ActionKind);
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

        clock.Set(before.TurnDeadlineUtc!.Value);
        Assert.Equal(before.CurrentTurnPlayerId, game.TryExpireTurn(matchId));
        clock.Set(game.GetBoard(matchId)!.PresenceDeadlinesUtc[before.CurrentTurnPlayerId]);
        Assert.False(game.TryConfirmPresence(matchId, before.CurrentTurnPlayerId));
        Assert.Equal(MatchReconnectResult.Rejected, game.TryReconnect(matchId, before.CurrentTurnPlayerId));
        Assert.Equal(before.CurrentTurnPlayerId, game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.CurrentTurnPlayerId, Assert.Single(after.InactivePlayerIds));
        Assert.Empty(after.PresenceDeadlinesUtc);
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
        clock.Set(board.TurnDeadlineUtc!.Value.AddSeconds(-1));
        Assert.Null(game.TryExpireTurn(matchId));
        clock.Set(board.TurnDeadlineUtc!.Value);

        Assert.Equal(board.CurrentTurnPlayerId, game.TryExpireTurn(matchId));
        MatchBoardSnapshot skipped = game.GetBoard(matchId)!;
        Assert.Equal(MatchEndReason.None, skipped.EndReason);
        Assert.Equal(MatchLifecycleStatus.Suspended, game.Get(matchId)!.Status);
        clock.Set(skipped.PresenceDeadlinesUtc[board.CurrentTurnPlayerId]);
        Assert.Equal(board.CurrentTurnPlayerId, game.TryExpireTurn(matchId));
        Assert.Equal(MatchEndReason.Abandonment, game.GetBoard(matchId)!.EndReason);
    }

    [Fact]
    public void TryExpireTurn_ConcurrentLegalActionAtDeadline_OnlySkipTakesEffect()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Func<bool> playTurn = GetLegalTurn(game, before);
        bool played = false;
        Guid? skipped = null;
        clock.Set(before.TurnDeadlineUtc!.Value);

        Parallel.Invoke(() => played = playTurn(), () => skipped = game.TryExpireTurn(matchId));

        Assert.False(played);
        Assert.Equal(before.CurrentTurnPlayerId, skipped);
        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(MatchBoardActionKind.TurnSkipped, Assert.Single(after.RecentMoves).ActionKind);
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
        clock.Set(before.TurnDeadlineUtc!.Value.AddSeconds(-1));

        Assert.True(playTurn());
        clock.Set(before.TurnDeadlineUtc!.Value);
        Assert.Null(game.TryExpireTurn(matchId));

        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(before.TurnDeadlineUtc!.Value.AddSeconds(-1) + (before.TurnDeadlineUtc.Value - StartTime),
            after.TurnDeadlineUtc);
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Empty(after.ExpelledPlayerIds);
    }

    [Fact]
    public void TryExpireTurn_ExpelledHand_ReturnsAfterExistingModules()
    {
        Guid[] players = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        MatchSessionSnapshot session = new()
        {
            MatchId = Guid.NewGuid(),
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            InitialHostPlayerId = players[0],
            PlayerIds = players,
            CreatedAtUtc = StartTime.UtcDateTime
        };
        int[] modules = Enumerable.Range(1, 10).Concat(Enumerable.Range(21, 10))
            .Concat(Enumerable.Range(41, 10)).Concat(new[] { 90, 91, 92, 93, 95 }).ToArray();
        MatchRecord record = new(session, StartTime, () => new MatchDeck(modules));
        MatchBoardSnapshot board = record.ToBoard();
        int expectedReturnedModule = record.ToPrivateHand(board.CurrentTurnPlayerId)!.Cards[0];
        DateTimeOffset now = board.TurnDeadlineUtc!.Value.AddSeconds(1);

        Assert.Equal(board.CurrentTurnPlayerId, record.TryExpireTurn(now));
        now = record.ToBoard().PresenceDeadlinesUtc[board.CurrentTurnPlayerId];
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

    [Fact]
    public void TryReconnect_DuringGrace_KeepsSeatAndDeadlineUntilPresenceIsConfirmed()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid active = before.CurrentTurnPlayerId;
        Assert.True(game.TryDisconnect(matchId, active));
        clock.Set(before.TurnDeadlineUtc!.Value.AddSeconds(2));
        Assert.Equal(active, game.TryExpireTurn(matchId));
        MatchBoardSnapshot skipped = game.GetBoard(matchId)!;
        Assert.False(game.TryConfirmPresence(matchId, active));

        Assert.Equal(MatchReconnectResult.Reconnected, game.TryReconnect(matchId, active));
        Assert.Equal(before.TurnDeadlineUtc.Value.AddSeconds(30),
            game.GetBoard(matchId)!.PresenceDeadlinesUtc[active]);
        Assert.True(game.TryConfirmPresence(matchId, active));
        Assert.False(game.TryConfirmPresence(matchId, active));

        MatchBoardSnapshot confirmed = game.GetBoard(matchId)!;
        Assert.Equal(skipped.CurrentTurnPlayerId, confirmed.CurrentTurnPlayerId);
        Assert.Equal(skipped.TurnDeadlineUtc, confirmed.TurnDeadlineUtc);
        Assert.Empty(confirmed.PresenceDeadlinesUtc);
        Assert.Equal(MatchBoardActionKind.PresenceConfirmed, confirmed.RecentMoves.Last().ActionKind);
        clock.Set(skipped.PresenceDeadlinesUtc[active]);
        Assert.Null(game.TryExpireTurn(matchId));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Grace_WhenTurnReturns_RequiresValidMoveWithinOriginalDeadline(bool play)
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 2);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        Guid skippedPlayer = before.CurrentTurnPlayerId;
        clock.Set(before.TurnDeadlineUtc!.Value);
        Assert.Equal(skippedPlayer, game.TryExpireTurn(matchId));
        MatchBoardSnapshot skipped = game.GetBoard(matchId)!;
        DateTimeOffset deadline = skipped.PresenceDeadlinesUtc[skippedPlayer];
        clock.Set(deadline.AddSeconds(-20));
        Assert.True(GetLegalTurn(game, skipped)());

        MatchBoardSnapshot returned = game.GetBoard(matchId)!;
        Assert.Equal(skippedPlayer, returned.CurrentTurnPlayerId);
        Assert.Equal(deadline, returned.TurnDeadlineUtc);
        Assert.False(game.TryConfirmPresence(matchId, skippedPlayer));
        Assert.Equal(MatchPlacementResult.CardNotOwned,
            game.TryPlace(matchId, skippedPlayer, new MatchPlacement(0, 1, returned.Revision)));
        Assert.Equal(deadline, game.GetBoard(matchId)!.PresenceDeadlinesUtc[skippedPlayer]);

        if (play)
        {
            clock.Set(deadline.AddTicks(-1));
            Assert.True(GetLegalTurn(game, returned)());
        }

        clock.Set(deadline);
        Assert.Equal(play ? (Guid?)null : skippedPlayer, game.TryExpireTurn(matchId));
        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Empty(after.PresenceDeadlinesUtc);
        Assert.Equal(!play, after.InactivePlayerIds.Contains(skippedPlayer));
    }

    [Fact]
    public void Grace_ExpiresDuringAnotherTurn_WithoutChangingThatDeadline()
    {
        ManualClock clock = new();
        var (game, matchId) = StartMatch(clock, 3);
        MatchBoardSnapshot before = game.GetBoard(matchId)!;
        clock.Set(before.TurnDeadlineUtc!.Value);
        Assert.Equal(before.CurrentTurnPlayerId, game.TryExpireTurn(matchId));
        MatchBoardSnapshot skipped = game.GetBoard(matchId)!;
        DateTimeOffset deadline = skipped.PresenceDeadlinesUtc[before.CurrentTurnPlayerId];
        clock.Set(deadline.AddSeconds(-20));
        Assert.True(GetLegalTurn(game, skipped)());
        MatchBoardSnapshot next = game.GetBoard(matchId)!;

        clock.Set(deadline);
        Guid inactive = before.CurrentTurnPlayerId;
        Guid active = next.CurrentTurnPlayerId;
        Assert.False(game.TryConfirmPresence(matchId, inactive));
        Assert.Equal(MatchExpulsionVoteResult.TurnExpired,
            game.TryVoteToExpel(matchId, inactive, new MatchExpulsionVote(active, next.VotingRevision)));
        Assert.Equal(MatchExpulsionVoteResult.TurnExpired,
            game.TryVoteToExpel(matchId, active, new MatchExpulsionVote(inactive, next.VotingRevision)));
        Assert.Equal(next.Revision, game.GetBoard(matchId)!.Revision);
        Assert.Equal(inactive, game.TryExpireTurn(matchId));
        MatchBoardSnapshot after = game.GetBoard(matchId)!;
        Assert.Equal(next.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(next.TurnDeadlineUtc, after.TurnDeadlineUtc);
        Assert.Equal(before.CurrentTurnPlayerId, Assert.Single(after.InactivePlayerIds));
        Assert.Null(game.TryExpireTurn(matchId));
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
