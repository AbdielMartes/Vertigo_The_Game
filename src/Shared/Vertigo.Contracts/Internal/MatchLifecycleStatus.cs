namespace Vertigo.Contracts.Internal;

// Internal game lifecycle, independent of LobbyStatus and CoreWCF.
public enum MatchLifecycleStatus
{
    Running,
    Suspended,
    Finished,
    Cancelled
}
