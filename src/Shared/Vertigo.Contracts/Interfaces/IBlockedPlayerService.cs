using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IBlockedPlayerService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool BlockPlayer(Guid otherPlayerId, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
