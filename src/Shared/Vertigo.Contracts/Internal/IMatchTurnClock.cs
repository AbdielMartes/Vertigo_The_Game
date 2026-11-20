namespace Vertigo.Contracts.Internal;

public interface IMatchTurnClock
{
    // Advances one elapsed deadline and returns the affected player.
    Guid? TryExpireTurn(Guid matchId);
}
