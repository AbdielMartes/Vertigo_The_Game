using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IFriendshipService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendEntry> GetFriends(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
