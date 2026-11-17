namespace Vertigo.Contracts.Internal;

public sealed record MatchPresenceSnapshot
{
    public required Guid MatchId { get; init; }

    public required MatchLifecycleStatus Status { get; init; }

    public required long Revision { get; init; }

    public required IReadOnlyList<Guid> DisconnectedPlayerIds { get; init; }
}
