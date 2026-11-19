namespace Vertigo.Contracts.Internal;

public interface IMatchTurnClock
{
    Guid? TryExpireTurn(Guid matchId);
}
