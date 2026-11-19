using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public sealed class FriendshipQueries
{
    private const int MinimumSearchLength = 3;
    private const int MaximumSearchResults = 20;

    private readonly IPlayerRepository _playerRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;

    public FriendshipQueries(
        IPlayerRepository playerRepository,
        IFriendshipRepository friendshipRepository,
        IBlockedPlayerRepository blockedPlayerRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
        _blockedPlayerRepository = blockedPlayerRepository;
    }

    internal List<FriendEntry> GetFriendsForPlayer(Guid playerId)
    {
        List<FriendEntry> entries = new List<FriendEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);

        foreach (Friendship friendship in friendships)
        {
            entries.Add(CreateFriendEntry(playerId, friendship));
        }

        return entries.OrderBy(entry => entry.Username).ToList();
    }

    internal List<PlayerSearchEntry> SearchPlayersForPlayer(PlayerSearch request)
    {
        List<PlayerSearchEntry> entries = new List<PlayerSearchEntry>();

        EnsureSearchRequestIsValid(request);

        string searchTerm = request.SearchTerm.Trim();
        List<Player> players = _playerRepository.SearchPlayersByUsername(
            searchTerm,
            request.RequesterId,
            MaximumSearchResults);
        List<Guid> playerIds = players.Select(player => player.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.RequesterId, playerIds);
        List<Friendship> friendships = _friendshipRepository.GetFriendshipsBetween(request.RequesterId, playerIds);

        foreach (Player player in players)
        {
            bool hasBlockedRequester = blocks.Any(block => block.PlayerBlockerId == player.PlayerId);

            if (!hasBlockedRequester)
            {
                PlayerPair pair = new() { PlayerId = request.RequesterId, OtherPlayerId = player.PlayerId };
                PlayerRelation relation = ResolveRelation(pair, blocks, friendships);
                entries.Add(CreateSearchEntry(player, relation));
            }
        }

        return entries;
    }

    internal List<FriendshipInvitationEntry> GetPendingRequestsForPlayer(Guid playerId)
    {
        List<FriendshipInvitationEntry> entries = new List<FriendshipInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<Friendship> pendingFriendships = _friendshipRepository.GetPendingRequestsReceivedBy(playerId);

        foreach (Friendship friendship in pendingFriendships)
        {
            entries.Add(CreateRequestEntry(friendship));
        }

        return entries;
    }

    internal Player FindRegisteredPlayer(Guid playerId)
    {
        Player? player = _playerRepository.GetPlayerById(playerId);

        if (player is null)
        {
            throw CreateFault(ServiceErrorCodes.UserNotFound, ServiceMessageKeys.UserNotFound);
        }

        return player;
    }

    internal void EnsurePlayersAreNotBlocked(FriendshipInvitation request)
    {
        List<Guid> targetIds = new List<Guid> { request.TargetId };
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.SenderId, targetIds);

        if (blocks.Count > 0)
        {
            throw CreateFault(ServiceErrorCodes.FriendshipBlocked, ServiceMessageKeys.FriendshipBlocked);
        }
    }

    private static PlayerRelation ResolveRelation(
        PlayerPair pair,
        List<BlockedPlayer> blocks,
        List<Friendship> friendships)
    {
        PlayerRelation relation = PlayerRelation.None;

        bool isBlockedByRequester = blocks.Any(
            block => block.PlayerBlockerId == pair.PlayerId && block.PlayerBlockedId == pair.OtherPlayerId);
        Friendship? friendship = friendships.FirstOrDefault(
            candidate => candidate.PlayerRequesterId == pair.OtherPlayerId
                || candidate.PlayerAddresseeId == pair.OtherPlayerId);

        if (isBlockedByRequester)
        {
            relation = PlayerRelation.Blocked;
        }
        else if (friendship is not null && friendship.Status == FriendshipStatus.Accepted)
        {
            relation = PlayerRelation.Friends;
        }
        else if (friendship is not null && friendship.PlayerRequesterId == pair.PlayerId)
        {
            relation = PlayerRelation.RequestSent;
        }

        return relation;
    }

    private static PlayerSearchEntry CreateSearchEntry(Player player, PlayerRelation relation)
    {
        return new PlayerSearchEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Relation = relation
        };
    }

    private static FriendshipInvitationEntry CreateRequestEntry(Friendship friendship)
    {
        return new FriendshipInvitationEntry
        {
            SenderId = friendship.PlayerRequesterId,
            SenderUsername = friendship.PlayerRequester.Username
        };
    }

    private static void EnsureSearchRequestIsValid(PlayerSearch request)
    {
        bool isValid = request is not null
                       && request.RequesterId != Guid.Empty
                       && request.SearchTerm is not null
                       && request.SearchTerm.Trim().Length >= MinimumSearchLength;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FriendEntry CreateFriendEntry(Guid playerId, Friendship friendship)
    {
        Player friend = friendship.PlayerRequesterId == playerId
            ? friendship.PlayerAddressee
            : friendship.PlayerRequester;

        return CreateFriendEntry(friend);
    }

    internal static FriendEntry CreateFriendEntry(Player player)
    {
        return new FriendEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username
        };
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
