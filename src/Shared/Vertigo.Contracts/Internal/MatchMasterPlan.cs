namespace Vertigo.Contracts.Internal;

public sealed record MatchMasterPlan(IReadOnlyList<int> CardNumbers, int PilePosition, long ExpectedRevision);
