namespace Vertigo.Contracts.Internal;

public interface IMatchDismantleService
{
    MatchDismantleResult TryDismantle(Guid matchId, Guid playerId, MatchDismantle move);
}
