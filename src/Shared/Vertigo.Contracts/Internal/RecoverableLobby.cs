using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record RecoverableLobby
{
    public required IReadOnlyList<LobbyMember> Members { get; init; }
    public required bool IsPublic { get; init; }
    public required int MaxPlayers { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
}
