using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyClient : IDisposable
{
    event EventHandler<LobbyMember>? PlayerJoined;

    event EventHandler<Guid>? PlayerLeft;

    event EventHandler<LobbyChatMessage>? MessageReceived;

    string CreateLobby(bool isPublic, int maxPlayers, string sessionToken);

    LobbyAdmission JoinLobby(string lobbyCode, string sessionToken);

    bool LeaveLobby(string lobbyCode, string sessionToken);

    List<LobbySummary> GetPublicLobbies();

    void ReplayPendingEvents();

    void Disconnect();
}
