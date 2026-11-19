namespace Vertigo.Contracts.Internal;

public enum MatchDismantleResult
{
    Applied,
    MatchUnavailable,
    StaleRevision,
    NotYourTurn,
    InvalidPile,
    EmptyPile,
    PlacementAvailable
}
