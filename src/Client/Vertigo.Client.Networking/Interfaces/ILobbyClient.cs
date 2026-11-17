using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyClient : IDisposable
{
    event EventHandler<LobbyMember>? PlayerJoined;

    event EventHandler<Guid>? PlayerLeft;

    event EventHandler<LobbyChatMessage>? MessageReceived;

    event EventHandler<LobbyPreparation>? PreparationChanged;

    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    bool LeaveLobby(string lobbyCode, string sessionToken);

    List<LobbySummary> GetPublicLobbies();

    LobbyPreparation GetPreparation(string lobbyCode, string sessionToken);

    LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken);

    LobbyPreparation StartMatch(string lobbyCode, string sessionToken);

    void ReplayPendingEvents();

    void Disconnect();
}
