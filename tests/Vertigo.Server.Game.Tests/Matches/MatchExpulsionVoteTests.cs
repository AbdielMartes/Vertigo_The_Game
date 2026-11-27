using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Game.Matches.Implementations;
using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchExpulsionVoteTests
{
    private static readonly DateTimeOffset Start = DateTimeOffset.UnixEpoch;

    [Fact]
    public void TryVoteToExpel_ThreePlayers_RequiresTwoDistinctVotersAndStartsNextTurn()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot initial = StartMatch(game, 3);
        Guid target = initial.CurrentTurnPlayerId;
        Guid[] playerOrder = initial.Players.Select(player => player.PlayerId).ToArray();
        Guid next = playerOrder[(Array.IndexOf(playerOrder, target) + 1) % playerOrder.Length];
        Guid other = playerOrder.Single(playerId => playerId != target && playerId != next);
        MatchExpulsionVote vote = new(target, initial.VotingRevision);
        clock.Now = Start.AddSeconds(7);

        Assert.Equal(2, initial.ExpulsionVotesRequired);
        Assert.Equal(MatchExpulsionVoteResult.InvalidTarget, game.TryVoteToExpel(initial.MatchId, target, vote));
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(initial.MatchId, next, vote));
        Assert.Equal(MatchExpulsionVoteResult.AlreadyVoted, game.TryVoteToExpel(initial.MatchId, next, vote));

        MatchBoardSnapshot pending = game.GetBoard(initial.MatchId)!;
        MatchExpulsionVoteSummary count = Assert.Single(pending.ExpulsionVotes);
        Assert.Equal(target, count.TargetPlayerId);
        Assert.Equal(1, count.VoteCount);
        Assert.Equal(initial.Revision + 1, pending.Revision);
        Assert.Equal(initial.TurnDeadlineUtc, pending.TurnDeadlineUtc);
        Assert.Empty(pending.RecentMoves);
        MatchPrivateHand votedHand = game.GetPrivateHand(initial.MatchId, next)!;
        Assert.Equal(pending.VotingRevision, votedHand.VotingRevision);
        Assert.Equal(target, Assert.Single(votedHand.VotedExpulsionTargetIds));
        Assert.Empty(game.GetPrivateHand(initial.MatchId, other)!.VotedExpulsionTargetIds);
        Assert.Equal(MatchExpulsionVoteResult.Expelled, game.TryVoteToExpel(initial.MatchId, other, vote));

        MatchBoardSnapshot after = game.GetBoard(initial.MatchId)!;
        Assert.Equal(next, after.CurrentTurnPlayerId);
        Assert.Equal(clock.Now + (initial.TurnDeadlineUtc!.Value - Start), after.TurnDeadlineUtc);
        Assert.Equal(initial.Revision + 2, after.Revision);
        Assert.Equal(target, Assert.Single(after.ExpelledPlayerIds));
        Assert.Equal(target, Assert.Single(after.ExpelledByVotePlayerIds));
        Assert.Equal(MatchBoardActionKind.ExpelledByVote, Assert.Single(after.RecentMoves).ActionKind);
        Assert.Empty(after.ExpulsionVotes);
        Assert.Null(game.GetPrivateHand(initial.MatchId, target));
        MatchPrivateHand afterHand = game.GetPrivateHand(initial.MatchId, next)!;
        Assert.Equal(after.VotingRevision, afterHand.VotingRevision);
        Assert.Empty(afterHand.VotedExpulsionTargetIds);
    }

    [Fact]
    public void TryVoteToExpel_FourPlayers_RequiresThreeAndPreservesAnotherPlayersTurn()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot initial = StartMatch(game, 4);
        Guid target = initial.Players.First(player => player.PlayerId != initial.CurrentTurnPlayerId).PlayerId;
        Guid[] voters = initial.Players.Select(player => player.PlayerId).Where(id => id != target).ToArray();
        MatchExpulsionVote vote = new(target, initial.VotingRevision);
        clock.Now = Start.AddSeconds(12);

        Assert.Equal(3, initial.ExpulsionVotesRequired);
        MatchExpulsionVote oldVoteForAnotherPlayer = new(voters[1], initial.VotingRevision);
        Assert.Equal(MatchExpulsionVoteResult.Recorded,
            game.TryVoteToExpel(initial.MatchId, voters[0], oldVoteForAnotherPlayer));
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(initial.MatchId, voters[0], vote));
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(initial.MatchId, voters[1], vote));
        Assert.Empty(game.GetBoard(initial.MatchId)!.ExpelledPlayerIds);
        Assert.Equal(MatchExpulsionVoteResult.Expelled, game.TryVoteToExpel(initial.MatchId, voters[2], vote));

        MatchBoardSnapshot after = game.GetBoard(initial.MatchId)!;
        Assert.Equal(initial.CurrentTurnPlayerId, after.CurrentTurnPlayerId);
        Assert.Equal(initial.TurnDeadlineUtc, after.TurnDeadlineUtc);
        Assert.Equal(initial.VotingRevision + 1, after.VotingRevision);
        Assert.Empty(after.ExpulsionVotes);

        Assert.Equal(MatchExpulsionVoteResult.StaleVotingRevision,
            game.TryVoteToExpel(initial.MatchId, voters[0], oldVoteForAnotherPlayer));
        Assert.Equal(MatchExpulsionVoteResult.InvalidVoter,
            game.TryVoteToExpel(initial.MatchId, target, new MatchExpulsionVote(voters[1], after.VotingRevision)));
    }

    [Fact]
    public void TryVoteToExpel_ConcurrentVotesWithSameRevision_ExpelsExactlyOnce()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot board = StartMatch(game, 3);
        Guid target = board.CurrentTurnPlayerId;
        Guid[] voters = board.Players.Select(player => player.PlayerId).Where(id => id != target).ToArray();
        MatchExpulsionVote vote = new(target, board.VotingRevision);
        MatchExpulsionVoteResult[] results = new MatchExpulsionVoteResult[voters.Length];

        Parallel.For(0, voters.Length, index =>
        {
            results[index] = game.TryVoteToExpel(board.MatchId, voters[index], vote);
        });

        Assert.Single(results, result => result == MatchExpulsionVoteResult.Recorded);
        Assert.Single(results, result => result == MatchExpulsionVoteResult.Expelled);
        MatchBoardSnapshot after = game.GetBoard(board.MatchId)!;
        Assert.Equal(board.Revision + 2, after.Revision);
        Assert.Equal(board.VotingRevision + 1, after.VotingRevision);
        Assert.Equal(target, Assert.Single(after.ExpelledPlayerIds));
        Assert.Equal(MatchBoardActionKind.ExpelledByVote, Assert.Single(after.RecentMoves).ActionKind);
        Assert.Empty(after.ExpulsionVotes);
    }

    [Fact]
    public void TryVoteToExpel_TwoPlayers_CannotReachMajority()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot board = StartMatch(game, 2);
        Guid voter = board.Players[0].PlayerId;
        Guid target = board.Players[1].PlayerId;

        MatchExpulsionVoteResult result = game.TryVoteToExpel(
            board.MatchId, voter, new MatchExpulsionVote(target, board.VotingRevision));

        Assert.Equal(MatchExpulsionVoteResult.TooFewPlayers, result);
        Assert.Equal(board.Revision, game.GetBoard(board.MatchId)!.Revision);
        Assert.Empty(game.GetBoard(board.MatchId)!.ExpulsionVotes);
    }

    [Fact]
    public void TryVoteToExpel_AtGraceExpiry_RecordsOneExpulsionAndClearsPreviousVotes()
    {
        ManualClock clock = new();
        InMemoryMatchSessionStarter game = new(clock);
        MatchBoardSnapshot board = StartMatch(game, 3);
        Guid target = board.CurrentTurnPlayerId;
        Guid[] voters = board.Players.Select(player => player.PlayerId).Where(id => id != target).ToArray();
        MatchExpulsionVote vote = new(target, board.VotingRevision);
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[0], vote));
        clock.Now = board.TurnDeadlineUtc!.Value;
        Assert.Equal(target, game.TryExpireTurn(board.MatchId));
        clock.Now = game.GetBoard(board.MatchId)!.PresenceDeadlinesUtc[target];
        MatchExpulsionVoteResult result = default;
        Guid? expired = null;

        Parallel.Invoke(
            () => result = game.TryVoteToExpel(board.MatchId, voters[1], vote),
            () => expired = game.TryExpireTurn(board.MatchId));

        Assert.True(result is MatchExpulsionVoteResult.TurnExpired or MatchExpulsionVoteResult.StaleVotingRevision);
        Assert.Equal(target, expired);
        MatchBoardSnapshot after = game.GetBoard(board.MatchId)!;
        Assert.Equal(board.Revision + 3, after.Revision);
        Assert.Equal(target, Assert.Single(after.ExpelledPlayerIds));
        Assert.Equal(MatchBoardActionKind.TurnExpired, after.RecentMoves.Last().ActionKind);
        Assert.Empty(after.ExpulsionVotes);
    }

    [Fact]
    public void TryPlay_WhenOnlyAVoteChangedRevision_AcceptsTheCurrentTurnAction()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot board = StartMatch(game, 3);
        Guid active = board.CurrentTurnPlayerId;
        Guid voter = board.Players.First(player => player.PlayerId != active).PlayerId;
        MatchPrivateHand hand = game.GetPrivateHand(board.MatchId, active)!;
        (int Card, int Pile) position = (from card in hand.Cards
                                        from pile in board.Piles
                                        where pile.TopCardNumber is int top && MatchPlacementRules.IsValid(top, card)
                                        select (card, pile.Position)).FirstOrDefault();
        Assert.Equal(MatchExpulsionVoteResult.Recorded,
            game.TryVoteToExpel(board.MatchId, voter, new MatchExpulsionVote(active, board.VotingRevision)));

        if (position.Pile == 0)
        {
            MatchDismantle move = new(1, board.Revision);
            Assert.Equal(MatchDismantleResult.Applied, game.TryDismantle(board.MatchId, active, move));
            Assert.Equal(MatchDismantleResult.StaleRevision, game.TryDismantle(board.MatchId, active, move));
        }
        else
        {
            MatchPlacement move = new(position.Card, position.Pile, board.Revision);
            Assert.Equal(MatchPlacementResult.Applied, game.TryPlace(board.MatchId, active, move));
            Assert.Equal(MatchPlacementResult.StaleRevision, game.TryPlace(board.MatchId, active, move));
        }

        Assert.Equal(board.Revision + 2, game.GetBoard(board.MatchId)!.Revision);
    }

    [Fact]
    public void TryVoteToExpel_RejectionPreventsMajorityAndOldResponseCannotVoteInNewBallot()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot board = StartMatch(game, 3);
        Guid target = board.CurrentTurnPlayerId;
        Guid[] voters = board.Players.Select(player => player.PlayerId).Where(id => id != target).ToArray();
        MatchExpulsionVote request = new(target, board.VotingRevision, MatchExpulsionDecision.Request);

        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[0], request));
        MatchExpulsionVoteSummary open = Assert.Single(game.GetBoard(board.MatchId)!.ExpulsionVotes);
        MatchExpulsionVote reject = new(target, board.VotingRevision, MatchExpulsionDecision.Reject, open.BallotId);
        Assert.Equal(MatchExpulsionVoteResult.InvalidTarget, game.TryVoteToExpel(board.MatchId, target, reject));
        Assert.Equal(MatchExpulsionVoteResult.AlreadyVoted, game.TryVoteToExpel(board.MatchId, voters[0], reject));
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[1], reject));

        MatchBoardSnapshot after = game.GetBoard(board.MatchId)!;
        MatchExpulsionVoteSummary closed = Assert.Single(after.ExpulsionVotes);
        Assert.True(closed.IsRejected);
        Assert.Equal(1, closed.VoteCount);
        Assert.Equal(1, closed.RejectedCount);
        Assert.Empty(after.ExpelledPlayerIds);
        Assert.Empty(after.RecentMoves);
        Assert.Equal(target, Assert.Single(game.GetPrivateHand(board.MatchId, voters[1])!.VotedExpulsionTargetIds));

        MatchExpulsionVote legacyVote = new(target, board.VotingRevision);
        Assert.Equal(MatchExpulsionVoteResult.StaleVotingRevision,
            game.TryVoteToExpel(board.MatchId, voters[0], request));
        Assert.Equal(MatchExpulsionVoteResult.StaleVotingRevision,
            game.TryVoteToExpel(board.MatchId, voters[1], legacyVote));
        Assert.Equal(after.Revision, game.GetBoard(board.MatchId)!.Revision);

        MatchExpulsionVote reopen = request with { BallotId = closed.BallotId };
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[0], reopen));
        Assert.Equal(MatchExpulsionVoteResult.StaleVotingRevision, game.TryVoteToExpel(board.MatchId, voters[1], reject));
        Assert.Equal(MatchExpulsionVoteResult.StaleVotingRevision,
            game.TryVoteToExpel(board.MatchId, voters[1], legacyVote));
        MatchExpulsionVoteSummary reopened = Assert.Single(game.GetBoard(board.MatchId)!.ExpulsionVotes);
        Assert.NotEqual(open.BallotId, reopened.BallotId);
        Assert.False(reopened.IsRejected);
        Assert.Equal(1, reopened.VoteCount);
        Assert.Equal(0, reopened.RejectedCount);
        MatchExpulsionVote accept = new(target, board.VotingRevision, MatchExpulsionDecision.Accept, reopened.BallotId);
        Assert.Equal(MatchExpulsionVoteResult.Expelled, game.TryVoteToExpel(board.MatchId, voters[1], accept));
    }

    [Fact]
    public void TryVoteToExpel_FivePlayers_StaysOpenWhenExactlyEnoughAcceptancesRemainPossible()
    {
        InMemoryMatchSessionStarter game = new(new ManualClock());
        MatchBoardSnapshot board = StartMatch(game, 5);
        Guid target = board.CurrentTurnPlayerId;
        Guid[] voters = board.Players.Select(player => player.PlayerId).Where(id => id != target).ToArray();
        Assert.Equal(3, board.ExpulsionVotesRequired);
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(
            board.MatchId, voters[0], new MatchExpulsionVote(target, board.VotingRevision, MatchExpulsionDecision.Request)));
        MatchExpulsionVoteSummary open = Assert.Single(game.GetBoard(board.MatchId)!.ExpulsionVotes);
        MatchExpulsionVote reject = new(target, board.VotingRevision, MatchExpulsionDecision.Reject, open.BallotId);

        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[1], reject));
        MatchExpulsionVoteSummary pending = Assert.Single(game.GetBoard(board.MatchId)!.ExpulsionVotes);
        Assert.False(pending.IsRejected);
        Assert.Equal(1, pending.RejectedCount);

        MatchExpulsionVote accept = reject with { Decision = MatchExpulsionDecision.Accept };
        Assert.Equal(MatchExpulsionVoteResult.Recorded, game.TryVoteToExpel(board.MatchId, voters[2], accept));
        Assert.Equal(MatchExpulsionVoteResult.Expelled, game.TryVoteToExpel(board.MatchId, voters[3], accept));
    }

    private static MatchBoardSnapshot StartMatch(InMemoryMatchSessionStarter game, int playerCount)
    {
        Guid[] players = Enumerable.Range(0, playerCount).Select(_ => Guid.NewGuid()).ToArray();
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
        public DateTimeOffset Now { get; set; } = Start;

        public override DateTimeOffset GetUtcNow()
        {
            return Now;
        }
    }
}
