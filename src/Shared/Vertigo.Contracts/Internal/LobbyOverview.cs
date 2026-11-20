using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record LobbyOverview
{
    public required string LobbyCode { get; init; }

    public required Guid HostPlayerId { get; init; }

    public required int MaxPlayers { get; init; }

    public required bool IsWaiting { get; init; }

    public required List<LobbyMember> Members { get; init; }

    public required Dictionary<Guid, Guid> Invitations { get; init; }
}
