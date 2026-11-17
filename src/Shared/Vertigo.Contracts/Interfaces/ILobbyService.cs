using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(ILobbyCallback), SessionMode = SessionMode.Required)]
public interface ILobbyService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool LeaveLobby(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbySummary> GetPublicLobbies();
}
