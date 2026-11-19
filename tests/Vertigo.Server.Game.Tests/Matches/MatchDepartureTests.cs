using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchDepartureTests
{
    [Fact]
    public void TryLeaveMatch_TwoPlayers_FinishesByAbandonmentAndAcceptsRetriesWithoutChangingBoard()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot before = StartMatch(game, 2);
        Guid leaving = before.Players[0].PlayerId;
        Guid remaining = before.Players[1].PlayerId;

        MatchDepartureResult departure = game.TryLeaveMatch(before.MatchId, leaving);

        Assert.Equal(new MatchDepartureResult(true, true), departure);
        MatchBoardSnapshot finished = game.GetBoard(before.MatchId)!;
        Assert.Equal(MatchEndReason.Abandonment, finished.EndReason);
        Assert.Equal(remaining, Assert.Single(finished.WinnerIds));
        Assert.Equal(leaving, Assert.Single(finished.ExpelledPlayerIds));
        Assert.Equal(MatchBoardActionKind.LeftMatch, Assert.Single(finished.RecentMoves).ActionKind);
        Assert.Empty(finished.RoundSummaries);
        Assert.Null(finished.TurnDeadlineUtc);
        Assert.Null(game.GetPrivateHand(before.MatchId, leaving));

        Assert.Equal(new MatchDepartureResult(true, false), game.TryLeaveMatch(before.MatchId, leaving));
        Assert.Equal(new MatchDepartureResult(true, false), game.TryLeaveMatch(before.MatchId, remaining));
        Assert.Equal(finished.Revision, game.GetBoard(before.MatchId)!.Revision);
        Assert.Equal(remaining, Assert.Single(game.GetBoard(before.MatchId)!.WinnerIds));
    }

    [Fact]
    public void TryLeaveMatch_WaitingPlayerLeaves_ContinuesCurrentTurnWithSameDeadline()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot before = StartMatch(game, 3);
        Guid leaving = before.Players.First(player => player.PlayerId != before.CurrentTurnPlayerId).PlayerId;
        clock.Now = clock.Now.AddSeconds(6);

        MatchDepartureResult departure = game.TryLeaveMatch(before.MatchId, leaving);

        Assert.Equal(new MatchDepartureResult(true, true), departure);
        MatchBoardSnapshot after = game.GetBoard(before.MatchId)!;
        Assert.Equal(MatchEndReason.None, after.EndReason);
        Assert.Equal(before.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(before.TurnDeadlineUtc, after.TurnDeadlineUtc);
        Assert.Equal(before.Revision + 1, after.Revision);
        Assert.Equal(2, after.Players.Count);
    }

    [Fact]
    public void TryLeaveMatch_CurrentPlayerWithPendingVote_AdvancesTurnAndClearsVotes()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot before = StartMatch(game, 3);
        Guid leaving = before.CurrentTurnPlayerId;
        Guid[] order = before.Players.Select(player => player.PlayerId).ToArray();
        Guid nextPlayer = order[(Array.IndexOf(order, leaving) + 1) % order.Length];
        MatchExpulsionVote vote = new(leaving, before.VotingRevision);
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(before.MatchId, nextPlayer, vote));
        clock.Now = clock.Now.AddSeconds(6);

        MatchDepartureResult departure = game.TryLeaveMatch(before.MatchId, leaving);

        Assert.Equal(new MatchDepartureResult(true, true), departure);
        MatchBoardSnapshot after = game.GetBoard(before.MatchId)!;
        Assert.Equal(nextPlayer, after.CurrentTurnPlayerId);
        Assert.Equal(clock.Now.AddSeconds(20), after.TurnDeadlineUtc);
        Assert.Equal(MatchEndReason.None, after.EndReason);
        Assert.Equal(before.Revision + 2, after.Revision);
        Assert.Equal(before.VotingRevision + 1, after.VotingRevision);
        Assert.Empty(after.ExpulsionVotes);
        Assert.Empty(game.GetPrivateHand(before.MatchId, nextPlayer)!.VotedExpulsionTargetIds);
        Assert.Equal(leaving, Assert.Single(after.ExpelledPlayerIds));
    }

    [Fact]
    public void TryLeaveMatch_WhenTurnAlreadyExpired_UsesTimeoutResultFirst()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot before = StartMatch(game, 2);
        Guid leaving = before.Players.Single(player => player.PlayerId != before.CurrentTurnPlayerId).PlayerId;
        clock.Now = before.TurnDeadlineUtc!.Value;

        Assert.Equal(new MatchDepartureResult(true, true), game.TryLeaveMatch(before.MatchId, leaving));

        MatchBoardSnapshot after = game.GetBoard(before.MatchId)!;
        Assert.Equal(leaving, Assert.Single(after.WinnerIds));
        Assert.Equal(MatchBoardActionKind.TurnExpired, Assert.Single(after.RecentMoves).ActionKind);
        Assert.Null(game.TryExpireTurn(before.MatchId));
    }

    [Fact]
    public void TryLeaveMatch_UnknownParticipant_DoesNotChangeGame()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot before = StartMatch(game, 2);

        Assert.Equal(new MatchDepartureResult(false, false), game.TryLeaveMatch(before.MatchId, Guid.NewGuid()));
        Assert.Equal(before.Revision, game.GetBoard(before.MatchId)!.Revision);
    }

    private static MatchBoardSnapshot StartMatch(InMemoryMatchSessionStarter game, int count)
    {
        Guid[] players = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray();
        MatchSessionSnapshot session = game.Start(new MatchStartRequest
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABC234",
            HostPlayerId = players[0],
            PlayerIds = players
        });

        return game.GetBoard(session.MatchId)!;
    }

    private sealed class ManualClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UnixEpoch;

        public override DateTimeOffset GetUtcNow()
        {
            return Now;
        }
    }
}
