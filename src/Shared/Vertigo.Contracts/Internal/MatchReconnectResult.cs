namespace Vertigo.Contracts.Internal;

// Result of a single atomic attempt inside the Game registry lock.
public enum MatchReconnectResult
{
    Reconnected,
    AlreadyConnected,
    Rejected
}
