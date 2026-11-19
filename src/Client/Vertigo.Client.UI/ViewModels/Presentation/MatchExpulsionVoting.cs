using System;
using System.Collections.Generic;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Presentation;

internal sealed class MatchExpulsionVoting
{
    private Guid? _localPlayerId;
    private HashSet<Guid>? _votedTargets;

    public MatchBoardSnapshot? Board { get; private set; }

    public long Revision => Board?.VotingRevision ?? 0;

    public int RequiredVotes => Board?.ExpulsionVotesRequired ?? 0;

    public void ApplyBoard(MatchBoardSnapshot? board, Guid? localPlayerId)
    {
        Board = board;
        _localPlayerId = localPlayerId;
        _votedTargets = null;
    }

    public void ApplyPrivateHand(MatchPrivateHand? hand)
    {
        _votedTargets = hand is not null && Board is not null && hand.MatchId == Board.MatchId
            && hand.PlayerId == _localPlayerId && hand.VotingRevision == Revision
            && hand.VotedExpulsionTargetIds is not null
            ? new HashSet<Guid>(hand.VotedExpulsionTargetIds) : null;
    }

    public int GetVoteCount(Guid targetId)
    {
        return Board?.ExpulsionVotes.Find(vote => vote.TargetPlayerId == targetId)?.VoteCount ?? 0;
    }

    public bool IsEligibleTarget(Guid targetId)
    {
        return Board is { EndReason: MatchEndReason.None, TurnDeadlineUtc: not null } board
            && RequiredVotes > 0 && board.Players.Count > RequiredVotes && _localPlayerId is Guid local
            && local != targetId && board.Players.Exists(player => player.PlayerId == local)
            && board.Players.Exists(player => player.PlayerId == targetId);
    }

    public bool CanVoteFor(Guid targetId)
    {
        return IsEligibleTarget(targetId) && _votedTargets is not null && !_votedTargets.Contains(targetId);
    }
}
