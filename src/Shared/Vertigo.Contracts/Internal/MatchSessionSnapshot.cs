namespace Vertigo.Contracts.Internal;

public sealed record MatchSessionSnapshot
{
    public required Guid MatchId { get; init; }

    public required Guid LobbyInstanceId { get; init; }

    public required string LobbyCode { get; init; }

    public required Guid InitialHostPlayerId { get; init; }

    public required IReadOnlyList<Guid> PlayerIds { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}
