namespace Vertigo.Contracts.Internal;

public interface IMatchCommandService :
    IMatchPlacementService,
    IMatchDismantleService,
    IMatchExpulsionVoteService,
    IMatchTurnClock,
    IMatchDepartureService,
    IMatchPresenceCommands
{
}
