namespace Vertigo.Contracts.Internal;

public interface IMatchDepartureService
{
    MatchDepartureResult TryLeaveMatch(Guid matchId, Guid playerId);
}
