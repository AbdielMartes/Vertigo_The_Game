namespace Vertigo.Contracts.Internal;

public sealed record MatchLifecycleSnapshot
{
    public required Guid MatchId { get; init; }

    public required Guid LobbyInstanceId { get; init; }

    public required MatchLifecycleStatus Status { get; init; }

    public required long Revision { get; init; }

    public required DateTimeOffset UpdatedAtUtc { get; init; }
}
