using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface IFriendshipNotifier
{
    void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId);

    void Unsubscribe(Guid playerId, string sessionId);

    void UnsubscribeSession(string sessionId);

    void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request);

    void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend);

    void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation);

    void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode);

    bool IsOnline(Guid playerId);
}
