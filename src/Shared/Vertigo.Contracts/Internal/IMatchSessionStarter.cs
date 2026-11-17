namespace Vertigo.Contracts.Internal;

// In-process contract: not a CoreWCF service and not a public endpoint.
public interface IMatchSessionStarter
{
    MatchSessionSnapshot Start(MatchStartRequest request);
}
