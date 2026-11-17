using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Contracts.Internal;
using CoreWCF;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

public class LobbyInvitationService : ILobbyInvitationService
{
    private const int MaximumInvitationsPerRequest = 5;

    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ILobbyDirectory _lobbyDirectory;
    private readonly IFriendshipNotifier _notifier;
    private readonly ISessionIdentityResolver _sessions;

    public LobbyInvitationService(LobbyInvitationServiceDependencies dependencies, ISessionIdentityResolver sessions)
    {
        _friendshipRepository = dependencies.FriendshipRepository;
        _blockedPlayerRepository = dependencies.BlockedPlayerRepository;
        _lobbyDirectory = dependencies.LobbyDirectory;
        _notifier = dependencies.Notifier;
        _sessions = sessions;
    }

    public List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        InvitableFriendsQuery query = new InvitableFriendsQuery
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return GetInvitableFriendsForPlayer(query);
    }

    public List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitations invitations = new LobbyInvitations
        {
            SenderId = identity.PlayerId,
            LobbyCode = lobbyCode,
            TargetIds = targetIds
        };

        return SendLobbyInvitationsForPlayer(invitations);
    }

    public List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetPendingInvitationsForPlayer(identity.PlayerId);
    }

    public bool DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitationDecline decline = new LobbyInvitationDecline
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return DeclineInvitationForPlayer(decline);
    }

    private List<FriendEntry> GetInvitableFriendsForPlayer(InvitableFriendsQuery request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyOverview lobby = FindLobbyOfMember(request!.LobbyCode, request.PlayerId);

        return FindInvitableFriends(request.PlayerId, lobby);
    }

    private List<Guid> SendLobbyInvitationsForPlayer(LobbyInvitations request)
    {
        List<Guid> invitedIds = new List<Guid>();

        EnsureInvitationRequestIsValid(request);

        LobbyOverview lobby = FindLobbyOfMember(request.LobbyCode, request.SenderId);
        LobbyMember sender = lobby.Members.First(member => member.PlayerId == request.SenderId);
        int freeSlots = lobby.MaxPlayers - lobby.Members.Count;

        if (!lobby.IsWaiting)
        {
            throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
        }

        if (sender.IsGuest)
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        if (freeSlots <= 0)
        {
            throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
        }

        List<FriendEntry> invitableFriends = FindInvitableFriends(request.SenderId, lobby)
            .Where(friend => request.TargetIds.Contains(friend.PlayerId))
            .ToList();

        foreach (FriendEntry friend in invitableFriends)
        {
            bool isRegistered = invitedIds.Count < freeSlots
                                && _lobbyDirectory.RegisterInvitation(lobby.LobbyCode, friend.PlayerId, sender.PlayerId);

            if (isRegistered)
            {
                invitedIds.Add(friend.PlayerId);
                _notifier.NotifyLobbyInvitationReceived(friend.PlayerId, CreateEntry(lobby, sender.Username, sender.PlayerId));
            }
        }

        return invitedIds;
    }

    private List<LobbyInvitationEntry> GetPendingInvitationsForPlayer(Guid playerId)
    {
        List<LobbyInvitationEntry> entries = new List<LobbyInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        foreach (LobbyOverview lobby in _lobbyDirectory.FindLobbiesInviting(playerId))
        {
            Guid senderId = lobby.Invitations[playerId];
            LobbyMember? sender = lobby.Members.FirstOrDefault(member => member.PlayerId == senderId)
                                  ?? lobby.Members.FirstOrDefault(member => member.PlayerId == lobby.HostPlayerId);

            entries.Add(CreateEntry(lobby, sender?.Username ?? string.Empty, senderId));
        }

        return entries;
    }

    private bool DeclineInvitationForPlayer(LobbyInvitationDecline request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        bool isDeclined = _lobbyDirectory.CancelInvitation(request!.LobbyCode, request.PlayerId);

        return isDeclined;
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

    private static void EnsureInvitationRequestIsValid(LobbyInvitations request)
    {
        bool isValid = request is not null
                       && request.SenderId != Guid.Empty
                       && request.TargetIds is not null
                       && request.TargetIds.Count > 0
                       && request.TargetIds.Count <= MaximumInvitationsPerRequest
                       && !request.TargetIds.Contains(request.SenderId);

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static LobbyInvitationEntry CreateEntry(LobbyOverview lobby, string senderUsername, Guid senderId)
    {
        return new LobbyInvitationEntry
        {
            LobbyCode = lobby.LobbyCode,
            SenderId = senderId,
            SenderUsername = senderUsername,
            CurrentPlayers = lobby.Members.Count,
            MaxPlayers = lobby.MaxPlayers
        };
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

    private LobbyOverview FindLobbyOfMember(string lobbyCode, Guid playerId)
    {
        LobbyOverview? lobby = _lobbyDirectory.FindLobby(lobbyCode);

        if (lobby is null)
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }

        if (lobby.Members.All(member => member.PlayerId != playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        return lobby;
    }

    private List<FriendEntry> FindInvitableFriends(Guid playerId, LobbyOverview lobby)
    {
        List<Friendship> friendships = _friendshipRepository.GetAcceptedFriendships(playerId);
        List<FriendEntry> friends = friendships
            .Select(friendship => CreateFriendEntry(playerId, friendship))
            .ToList();
        List<Guid> friendIds = friends.Select(friend => friend.PlayerId).ToList();
        List<BlockedPlayer> blocks = _blockedPlayerRepository.GetBlocksBetween(playerId, friendIds);

        List<FriendEntry> invitableFriends = friends
            .Where(friend => lobby.Members.All(member => member.PlayerId != friend.PlayerId))
            .Where(friend => !lobby.Invitations.ContainsKey(friend.PlayerId))
            .Where(friend => blocks.All(block => block.PlayerBlockerId != friend.PlayerId && block.PlayerBlockedId != friend.PlayerId))
            .Where(friend => _notifier.IsOnline(friend.PlayerId))
            .Where(friend => !_lobbyDirectory.IsInAnyLobby(friend.PlayerId))
            .OrderBy(friend => friend.Username)
            .ToList();

        return invitableFriends;
    }
}
