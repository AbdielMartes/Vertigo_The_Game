using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface IFriendshipCallback
{
    [OperationContract(IsOneWay = true)]
    void OnFriendRequestReceived(FriendshipInvitationEntry request);

    [OperationContract(IsOneWay = true)]
    void OnFriendshipEstablished(FriendEntry friend);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationReceived(LobbyInvitationEntry invitation);

    [OperationContract(IsOneWay = true)]
    void OnLobbyInvitationWithdrawn(string lobbyCode);
}
