using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

public sealed record PendingLobbyEvent(LobbyMember? Joined, Guid LeftPlayerId, LobbyChatMessage? Chat, LobbyPreparation? Preparation = null);
