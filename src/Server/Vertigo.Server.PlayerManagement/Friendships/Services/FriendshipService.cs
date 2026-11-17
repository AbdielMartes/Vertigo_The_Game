using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public class FriendshipService : IFriendshipService
{
    private const int MinimumSearchLength = 3;
    private const int MaximumSearchResults = 20;

    private readonly IPlayerRepository _playerRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public FriendshipService(FriendshipServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _playerRepository = dependencies.PlayerRepository;
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetFriends(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetFriendsForPlayer(identity.PlayerId);
    }

    public List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerSearch search = new PlayerSearch { RequesterId = identity.PlayerId, SearchTerm = searchTerm };
        return SearchPlayersForPlayer(search);
    }

    public PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipInvitation invitation = new FriendshipInvitation
        {
            SenderId = identity.PlayerId,
            TargetId = targetPlayerId
        };

        return SendFriendRequestForPlayer(invitation);
    }

    public List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingRequestsForPlayer(identity.PlayerId);
    }

    public bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipDecision decision = new FriendshipDecision
        {
            SenderId = senderPlayerId,
            ResponderId = identity.PlayerId,
            Accept = accept
        };

        return DecideFriendshipForPlayer(decision);
    }

    public bool RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerPair pair = new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId };
        return RemoveFriendshipForPlayer(pair);
    }

    private List<FriendEntry> GetFriendsForPlayer(Guid playerId)
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

    private List<PlayerSearchEntry> SearchPlayersForPlayer(PlayerSearch request)
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
                PlayerRelation relation = ResolveRelation(request.RequesterId, player.PlayerId, blocks, friendships);
                entries.Add(CreateSearchEntry(player, relation));
            }
        }

        return entries;
    }

    private PlayerRelation SendFriendRequestForPlayer(FriendshipInvitation request)
    {
        PlayerRelation relation = PlayerRelation.None;

        EnsureFriendRequestIsValid(request);

        Player sender = FindRegisteredPlayer(request.SenderId);
        Player target = FindRegisteredPlayer(request.TargetId);
        EnsurePlayersAreNotBlocked(request);

        Friendship? existingFriendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.TargetId);

        if (existingFriendship is null)
        {
            relation = CreatePendingFriendship(sender, target);
        }
        else if (existingFriendship.Status == FriendshipStatus.Accepted)
        {
            throw CreateFault(ServiceErrorCodes.AlreadyFriends, ServiceMessageKeys.AlreadyFriends);
        }
        else if (existingFriendship.PlayerRequesterId == request.SenderId)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestAlreadySent, ServiceMessageKeys.FriendRequestAlreadySent);
        }
        else
        {
            relation = AcceptCrossedRequest(existingFriendship, sender, target);
        }

        return relation;
    }

    private List<FriendshipInvitationEntry> GetPendingRequestsForPlayer(Guid playerId)
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

    private bool DecideFriendshipForPlayer(FriendshipDecision request)
    {
        bool isResponded = false;

        EnsureResponseIsValid(request);

        Friendship? friendship = _friendshipRepository.GetFriendshipBetween(request.SenderId, request.ResponderId);
        bool isPendingForResponder = friendship is not null
                                     && friendship.Status == FriendshipStatus.Pending
                                     && friendship.PlayerRequesterId == request.SenderId;

        if (!isPendingForResponder)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        if (request.Accept)
        {
            isResponded = AcceptFriendRequest(request);
        }
        else
        {
            isResponded = _friendshipRepository.DeletePendingFriendship(request.SenderId, request.ResponderId);
        }

        return isResponded;
    }

    private bool RemoveFriendshipForPlayer(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return _friendshipRepository.DeleteFriendshipBetween(pair.PlayerId, pair.OtherPlayerId);
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

    private static PlayerRelation ResolveRelation(
        Guid requesterId,
        Guid otherPlayerId,
        List<BlockedPlayer> blocks,
        List<Friendship> friendships)
    {
        PlayerRelation relation = PlayerRelation.None;

        bool isBlockedByRequester = blocks.Any(
            block => block.PlayerBlockerId == requesterId && block.PlayerBlockedId == otherPlayerId);
        Friendship? friendship = friendships.FirstOrDefault(
            candidate => candidate.PlayerRequesterId == otherPlayerId || candidate.PlayerAddresseeId == otherPlayerId);

        if (isBlockedByRequester)
        {
            relation = PlayerRelation.Blocked;
        }
        else if (friendship is not null && friendship.Status == FriendshipStatus.Accepted)
        {
            relation = PlayerRelation.Friends;
        }
        else if (friendship is not null && friendship.PlayerRequesterId == requesterId)
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

    private static void EnsureFriendRequestIsValid(FriendshipInvitation request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetId != Guid.Empty
                       && request.SenderId != request.TargetId;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static void EnsureResponseIsValid(FriendshipDecision request)
    {
        bool isValid = request is not null
                       && request.ResponderId != Guid.Empty
                       && request.SenderId != Guid.Empty
                       && request.ResponderId != request.SenderId;

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

    private static FriendEntry CreateFriendEntry(Player player)
    {
        return new FriendEntry
        {
            PlayerId = player.PlayerId,
            Username = player.Username
        };
    }

    private Player FindRegisteredPlayer(Guid playerId)
    {
        Player? player = _playerRepository.GetPlayerById(playerId);

        if (player is null)
        {
            throw CreateFault(ServiceErrorCodes.UserNotFound, ServiceMessageKeys.UserNotFound);
        }

        return player;
    }

    private void EnsurePlayersAreNotBlocked(FriendshipInvitation request)
    {
        List<Guid> targetIds = new List<Guid> { request.TargetId };
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(request.SenderId, targetIds);

        if (blocks.Count > 0)
        {
            throw CreateFault(ServiceErrorCodes.FriendshipBlocked, ServiceMessageKeys.FriendshipBlocked);
        }
    }

    private PlayerRelation CreatePendingFriendship(Player sender, Player target)
    {
        bool isCreated = _friendshipRepository.CreatePendingFriendship(sender.PlayerId, target.PlayerId);

        if (!isCreated)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        FriendshipInvitationEntry entry = new FriendshipInvitationEntry
        {
            SenderId = sender.PlayerId,
            SenderUsername = sender.Username
        };
        _notifier.NotifyFriendRequestReceived(target.PlayerId, entry);

        return PlayerRelation.RequestSent;
    }

    private PlayerRelation AcceptCrossedRequest(Friendship crossedFriendship, Player sender, Player target)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(
            crossedFriendship.PlayerRequesterId,
            crossedFriendship.PlayerAddresseeId);

        if (!isAccepted)
        {
            throw CreateFault(ServiceErrorCodes.FriendRequestNotFound, ServiceMessageKeys.FriendRequestNotFound);
        }

        _notifier.NotifyFriendshipEstablished(sender.PlayerId, CreateFriendEntry(target));
        _notifier.NotifyFriendshipEstablished(target.PlayerId, CreateFriendEntry(sender));

        return PlayerRelation.Friends;
    }

    private bool AcceptFriendRequest(FriendshipDecision request)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(request.SenderId, request.ResponderId);

        if (isAccepted)
        {
            Player responder = FindRegisteredPlayer(request.ResponderId);
            _notifier.NotifyFriendshipEstablished(request.SenderId, CreateFriendEntry(responder));
        }

        return isAccepted;
    }
}
