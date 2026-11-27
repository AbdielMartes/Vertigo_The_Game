using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record MatchExpulsionVote(
    Guid TargetPlayerId,
    long ExpectedVotingRevision,
    MatchExpulsionDecision Decision = MatchExpulsionDecision.Support,
    long BallotId = 0);
