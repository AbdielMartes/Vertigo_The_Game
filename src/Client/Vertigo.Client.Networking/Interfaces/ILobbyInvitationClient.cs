using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface ILobbyInvitationClient
{
    ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken);

    ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken);

    ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken);

    ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken);
}
