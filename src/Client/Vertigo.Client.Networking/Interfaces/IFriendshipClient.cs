using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipClient
{
    ClientResult<List<FriendEntry>> GetFriends(string sessionToken);

    ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken);

    ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken);

    ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken);

    ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken);

    ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken);
}
