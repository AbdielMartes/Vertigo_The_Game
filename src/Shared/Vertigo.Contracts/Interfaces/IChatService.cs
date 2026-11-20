using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IChatService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool SendMessage(string lobbyCode, string text, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool SendMatchMessage(MatchChatRequest request);
}
