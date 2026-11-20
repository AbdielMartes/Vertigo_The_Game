using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

public sealed record PendingLobbyEvent
{
    public LobbyMember? Joined { get; init; }

    public Guid LeftPlayerId { get; init; }

    public LobbyChatMessage? Chat { get; init; }

    public LobbyPreparation? Preparation { get; init; }
}
