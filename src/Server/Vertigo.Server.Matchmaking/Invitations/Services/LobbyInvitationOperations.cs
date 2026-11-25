using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

public sealed class LobbyInvitationOperations
{
    private const int MaximumInvitationsPerRequest = 5;

    private readonly LobbyInvitationFriendLookup _friendLookup;
    private readonly ILobbyDirectory _lobbyDirectory;
    private readonly IFriendshipNotifier _notifier;

    public LobbyInvitationOperations(
        LobbyInvitationFriendLookup friendLookup,
        ILobbyDirectory lobbyDirectory,
        IFriendshipNotifier notifier)
    {
        _friendLookup = friendLookup;
        _lobbyDirectory = lobbyDirectory;
        _notifier = notifier;
    }

    internal List<FriendEntry> GetInvitableFriendsForPlayer(InvitableFriendsQuery request)
    {
        bool isValid = request is not null && request.PlayerId != Guid.Empty;

        if (!isValid)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        LobbyOverview lobby = FindLobbyOfMember(request!.LobbyCode, request.PlayerId);

        return FindInvitableFriends(request.PlayerId, lobby);
    }

    internal List<Guid> SendLobbyInvitationsForPlayer(LobbyInvitations request)
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
                _notifier.NotifyLobbyInvitationReceived(friend.PlayerId, CreateEntry(lobby, sender, sender.PlayerId));
            }
        }

        return invitedIds;
    }

    internal List<LobbyInvitationEntry> GetPendingInvitationsForPlayer(Guid playerId)
    {
        List<LobbyInvitationEntry> entries = new List<LobbyInvitationEntry>();

        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        foreach (LobbyOverview lobby in _lobbyDirectory.FindLobbiesInviting(playerId))
        {
            Guid senderId = lobby.Invitations[playerId];
            LobbyMember? sender = lobby.Members.FirstOrDefault(member => member.PlayerId == senderId);

            entries.Add(CreateEntry(lobby, sender, senderId));
        }

        return entries;
    }

    internal bool DeclineInvitationForPlayer(LobbyInvitationDecline request)
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

    private static LobbyInvitationEntry CreateEntry(LobbyOverview lobby, LobbyMember? sender, Guid senderId)
    {
        return new LobbyInvitationEntry
        {
            LobbyCode = lobby.LobbyCode,
            SenderId = senderId,
            SenderUsername = sender?.Username ?? string.Empty,
            AvatarId = sender?.AvatarId,
            CurrentPlayers = lobby.Members.Count,
            MaxPlayers = lobby.MaxPlayers
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
        List<FriendEntry> friends = _friendLookup.FindUnblockedFriends(playerId);

        List<FriendEntry> invitableFriends = friends
            .Where(friend => lobby.Members.All(member => member.PlayerId != friend.PlayerId))
            .Where(friend => !lobby.Invitations.ContainsKey(friend.PlayerId))
            .Where(friend => _notifier.IsOnline(friend.PlayerId))
            .Where(friend => !_lobbyDirectory.IsInAnyLobby(friend.PlayerId))
            .OrderBy(friend => friend.Username)
            .ToList();

        return invitableFriends;
    }
}
