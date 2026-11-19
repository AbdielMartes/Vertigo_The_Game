using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyClient : IDisposable
{
    event EventHandler<LobbyMember>? PlayerJoined;

    event EventHandler<Guid>? PlayerLeft;

    event EventHandler<LobbyChatMessage>? MessageReceived;

    event EventHandler<LobbyPreparation>? PreparationChanged;

    event EventHandler<MatchBoardChanged>? MatchBoardUpdated;

    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    bool LeaveLobby(string lobbyCode, string sessionToken);

    bool LeaveMatch(MatchLeaveRequest request);

    List<LobbySummary> GetPublicLobbies();

    LobbyPreparation GetPreparation(string lobbyCode, string sessionToken);

    MatchBoardSnapshot GetMatchBoard(string lobbyCode, string sessionToken);

    MatchPrivateHand GetMatchPrivateHand(string lobbyCode, string sessionToken);

    MatchBoardSnapshot PlaceMatchCard(MatchPlaceRequest request);

    MatchBoardSnapshot DismantleMatchTower(MatchDismantleRequest request);

    MatchBoardSnapshot UseMatchMasterPlan(MatchMasterPlanRequest request);

    MatchBoardSnapshot VoteToExpelMatchPlayer(MatchExpulsionVoteRequest request);

    LobbyPreparation? GetCurrentLobby(string sessionToken);

    LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken);

    LobbyPreparation StartMatch(string lobbyCode, string sessionToken);

    void ReplayPendingEvents();

    void Disconnect();
}
