namespace Vertigo.Contracts.Internal;

public sealed record MatchSessionSnapshot(
    Guid MatchId,
    Guid LobbyInstanceId,
    string LobbyCode,
    Guid InitialHostPlayerId,
    IReadOnlyList<Guid> PlayerIds,
    DateTime CreatedAtUtc);
