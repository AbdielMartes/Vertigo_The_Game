namespace Vertigo.Contracts.Internal;

public interface IMatchPresenceCommands
{
    bool TryDisconnect(Guid matchId, Guid playerId);

    MatchReconnectResult TryReconnect(Guid matchId, Guid playerId);
}
