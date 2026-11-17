using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface ILobbyInvitationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
