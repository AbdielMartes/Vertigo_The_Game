namespace Vertigo.Contracts.Internal;

// In-process contract only. Identity is verified by Matchmaking before calling Game.
public interface IMatchPresence
{
    bool TryDisconnect(Guid matchId, Guid playerId);

    MatchReconnectResult TryReconnect(Guid matchId, Guid playerId);

    MatchPresenceSnapshot? GetPresence(Guid matchId);
}
