namespace Vertigo.Contracts.Internal;

public interface ICompletedMatchSource
{
    IReadOnlyList<CompletedMatch> GetPendingResults(int limit);
    void DeferResult(Guid matchId);
    void AcknowledgeResult(Guid matchId);
}
