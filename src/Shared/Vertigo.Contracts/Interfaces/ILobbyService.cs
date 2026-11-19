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
    bool LeaveMatch(MatchLeaveRequest request);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    List<LobbySummary> GetPublicLobbies();

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation GetPreparation(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchBoardSnapshot GetMatchBoard(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation? GetCurrentLobby(string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchBoardSnapshot PlaceMatchCard(MatchPlaceRequest request);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchBoardSnapshot DismantleMatchTower(MatchDismantleRequest request);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchBoardSnapshot UseMatchMasterPlan(MatchMasterPlanRequest request);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchBoardSnapshot VoteToExpelMatchPlayer(MatchExpulsionVoteRequest request);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    MatchPrivateHand GetMatchPrivateHand(string lobbyCode, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    LobbyPreparation StartMatch(string lobbyCode, string sessionToken);
}
