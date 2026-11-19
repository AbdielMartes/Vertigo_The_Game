namespace Vertigo.Contracts.Internal;

public sealed record MatchExpulsionVote(Guid TargetPlayerId, long ExpectedVotingRevision);
