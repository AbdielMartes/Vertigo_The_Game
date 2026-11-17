namespace Vertigo.Contracts.Internal;

public sealed record MatchStartRequest
{
    public required Guid LobbyInstanceId { get; init; }

    public required string LobbyCode { get; init; }

    public required Guid HostPlayerId { get; init; }

    public required IReadOnlyList<Guid> PlayerIds { get; init; }
}
