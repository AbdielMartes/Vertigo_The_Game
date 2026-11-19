namespace Vertigo.Contracts.Internal;

// In-process contract only. Identity is verified by Matchmaking before calling Game.
public interface IMatchPresence : IMatchPresenceCommands
{
    MatchPresenceSnapshot? GetPresence(Guid matchId);
}
