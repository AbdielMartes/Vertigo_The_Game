using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public sealed record LobbyOverview(
    string LobbyCode,
    Guid HostPlayerId,
    int MaxPlayers,
    bool IsWaiting,
    List<LobbyMember> Members,
    Dictionary<Guid, Guid> Invitations);
