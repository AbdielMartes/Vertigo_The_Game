using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Models;

internal sealed class MatchExpulsionBallot(long id, bool allowsLegacySupport)
{
    private readonly HashSet<Guid> _voters = new();

    public long Id { get; } = id;
    public bool AllowsLegacySupport { get; } = allowsLegacySupport;
    public int AcceptedCount { get; private set; }
    public int RejectedCount => _voters.Count - AcceptedCount;
    public bool IsRejected { get; private set; }

    public bool HasVoted(Guid voterId)
    {
        return _voters.Contains(voterId);
    }

    public bool TryRecord(Guid voterId, bool accepts, int eligibleVoters, int votesRequired)
    {
        bool recorded = _voters.Add(voterId);
        if (recorded)
        {
            AcceptedCount += accepts ? 1 : 0;
            IsRejected = eligibleVoters - RejectedCount < votesRequired;
        }

        return recorded;
    }

    public MatchExpulsionVoteSummary ToSummary(Guid targetId)
    {
        return new MatchExpulsionVoteSummary
        {
            TargetPlayerId = targetId,
            BallotId = Id,
            VoteCount = AcceptedCount,
            RejectedCount = RejectedCount,
            IsRejected = IsRejected
        };
    }
}
