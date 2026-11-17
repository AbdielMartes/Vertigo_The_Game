using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface ILobbyDirectory
{
    LobbyOverview? FindLobby(string lobbyCode);

    bool IsInAnyLobby(Guid playerId);

    bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId);

    bool CancelInvitation(string lobbyCode, Guid targetPlayerId);

    List<LobbyOverview> FindLobbiesInviting(Guid playerId);

    bool PublishChatMessage(LobbyChatMessage message);
}
