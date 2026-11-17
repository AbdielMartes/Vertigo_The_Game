namespace Vertigo.Contracts.Internal;

public interface IMatchPlacementService
{
    MatchPlacementResult TryPlace(Guid matchId, Guid playerId, MatchPlacement move);
}
