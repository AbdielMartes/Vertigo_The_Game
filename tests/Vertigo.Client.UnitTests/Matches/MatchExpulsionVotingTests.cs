using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchExpulsionVotingTests
{
    [Fact]
    public void CanVoteFor_ActiveLocalPlayer_CanVoteOutsideOwnTurnAndCannotVoteForSelf()
    {
        MatchExpulsionVoting voting = new();
        Guid localId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        MatchBoardSnapshot board = CreateBoard(localId, targetId, Guid.NewGuid());

        voting.ApplyBoard(board, localId);
        voting.ApplyPrivateHand(CreateHand(board, localId));

        Assert.NotEqual(localId, board.CurrentTurnPlayerId);
        Assert.True(voting.CanVoteFor(targetId));
        Assert.False(voting.CanVoteFor(localId));
        Assert.False(voting.CanVoteFor(Guid.NewGuid()));
    }

    [Fact]
    public void ApplyPrivateHand_PreviousVoteAfterReconnect_DisablesOnlyThatTarget()
    {
        MatchExpulsionVoting voting = new();
        Guid localId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        MatchBoardSnapshot board = CreateBoard(localId, targetId, otherId);
        board.ExpulsionVotes.Add(new MatchExpulsionVoteSummary { TargetPlayerId = targetId, VoteCount = 1 });

        voting.ApplyBoard(board, localId);
        voting.ApplyPrivateHand(CreateHand(board, localId, targetId));

        Assert.True(voting.IsEligibleTarget(targetId));
        Assert.False(voting.CanVoteFor(targetId));
        Assert.True(voting.CanVoteFor(otherId));
        Assert.Equal(1, voting.GetVoteCount(targetId));
        Assert.Equal(2, voting.RequiredVotes);
    }

    [Fact]
    public void CanVoteFor_TwoPlayersOrMatchFinished_IsUnavailable()
    {
        MatchExpulsionVoting voting = new();
        Guid localId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        MatchBoardSnapshot twoPlayers = CreateBoard(localId, targetId);
        voting.ApplyBoard(twoPlayers, localId);
        voting.ApplyPrivateHand(CreateHand(twoPlayers, localId));
        Assert.False(voting.IsEligibleTarget(targetId));
        Assert.False(voting.CanVoteFor(targetId));

        MatchBoardSnapshot finished = CreateBoard(localId, targetId, Guid.NewGuid());
        finished.EndReason = MatchEndReason.Completed;
        voting.ApplyBoard(finished, localId);
        voting.ApplyPrivateHand(CreateHand(finished, localId));
        Assert.False(voting.IsEligibleTarget(targetId));
        Assert.False(voting.CanVoteFor(targetId));
    }

    [Fact]
    public void ApplyPrivateHand_DifferentVotingRevision_WaitsForCurrentPrivateState()
    {
        MatchExpulsionVoting voting = new();
        Guid localId = Guid.NewGuid();
        Guid targetId = Guid.NewGuid();
        MatchBoardSnapshot board = CreateBoard(localId, targetId, Guid.NewGuid());
        board.VotingRevision = 1;
        MatchPrivateHand stale = CreateHand(board, localId);
        stale.VotingRevision = 0;

        voting.ApplyBoard(board, localId);
        voting.ApplyPrivateHand(stale);
        Assert.False(voting.CanVoteFor(targetId));

        voting.ApplyPrivateHand(CreateHand(board, localId));
        Assert.True(voting.CanVoteFor(targetId));
    }

    private static MatchBoardSnapshot CreateBoard(params Guid[] players)
    {
        return new MatchBoardSnapshot
        {
            MatchId = Guid.NewGuid(),
            CurrentTurnPlayerId = players[1],
            TurnDeadlineUtc = DateTimeOffset.UnixEpoch.AddSeconds(20),
            Players = players.Select(id => new MatchBoardPlayer { PlayerId = id }).ToList(),
            ExpulsionVotesRequired = players.Length / 2 + 1
        };
    }

    private static MatchPrivateHand CreateHand(MatchBoardSnapshot board, Guid playerId, params Guid[] votedTargets)
    {
        return new MatchPrivateHand
        {
            MatchId = board.MatchId,
            PlayerId = playerId,
            VotingRevision = board.VotingRevision,
            VotedExpulsionTargetIds = votedTargets.ToList()
        };
    }
}
