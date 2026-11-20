using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

public sealed class LobbyInvitationFriendLookup
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;

    public LobbyInvitationFriendLookup(
        IFriendshipRepository friendshipRepository,
        IBlockedPlayerRepository blockedPlayerRepository)
    {
        _friendshipRepository = friendshipRepository;
        _blockedPlayerRepository = blockedPlayerRepository;
    }

    internal List<FriendEntry> FindUnblockedFriends(Guid playerId)
    {
        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);
        List<FriendEntry> friends = friendships
            .Select(friendship => CreateFriendEntry(playerId, friendship))
            .ToList();
        List<Guid> friendIds = friends.Select(friend => friend.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(playerId, friendIds);

        return friends
            .Where(friend => blocks.All(block =>
                block.PlayerBlockerId != friend.PlayerId && block.PlayerBlockedId != friend.PlayerId))
            .ToList();
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return new FriendEntry
        {
            PlayerId = friend.PlayerId,
            Username = friend.Username
        };
    }
}
