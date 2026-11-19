namespace Vertigo.Contracts.Internal;

public enum MatchPlacementResult
{
    Applied,
    MatchUnavailable,
    NotYourTurn,
    CardNotOwned,
    InvalidPile,
    InvalidPlacement,
    StaleRevision,
    MasterPlanRequired,
    InvalidMasterPlanChain,
    TurnExpired
}
