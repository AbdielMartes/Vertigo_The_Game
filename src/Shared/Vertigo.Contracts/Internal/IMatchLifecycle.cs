namespace Vertigo.Contracts.Internal;

// In-process API only; deliberately not a CoreWCF endpoint.
public interface IMatchLifecycle
{
    MatchLifecycleSnapshot? Get(Guid matchId);

    bool TryFinish(Guid matchId);

    bool TryCancel(Guid matchId);

    int PurgeTerminated(TimeSpan retention);
}
