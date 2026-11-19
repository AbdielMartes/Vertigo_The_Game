using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public sealed class FriendshipCommands
{
    private readonly FriendshipQueries _queries;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendshipNotifier _notifier;

    public FriendshipCommands(
        FriendshipQueries queries,
        IFriendshipRepository friendshipRepository,
        IFriendshipNotifier notifier)
    {
        _queries = queries;
        _friendshipRepository = friendshipRepository;
        _notifier = notifier;
    }

    internal PlayerRelation SendFriendRequestForPlayer(FriendshipInvitation request)
    {
        PlayerRelation relation = PlayerRelation.None;

        EnsureFriendRequestIsValid(request);

        Player sender = _queries.FindRegisteredPlayer(request.SenderId);
        Player target = _queries.FindRegisteredPlayer(request.TargetId);
        _queries.EnsurePlayersAreNotBlocked(request);

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

    internal bool DecideFriendshipForPlayer(FriendshipDecision request)
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

    internal bool RemoveFriendshipForPlayer(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return _friendshipRepository.DeleteFriendshipBetween(pair.PlayerId, pair.OtherPlayerId);
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

        _notifier.NotifyFriendshipEstablished(sender.PlayerId, FriendshipQueries.CreateFriendEntry(target));
        _notifier.NotifyFriendshipEstablished(target.PlayerId, FriendshipQueries.CreateFriendEntry(sender));

        return PlayerRelation.Friends;
    }

    private bool AcceptFriendRequest(FriendshipDecision request)
    {
        bool isAccepted = _friendshipRepository.AcceptPendingFriendship(request.SenderId, request.ResponderId);

        if (isAccepted)
        {
            Player responder = _queries.FindRegisteredPlayer(request.ResponderId);
            _notifier.NotifyFriendshipEstablished(request.SenderId, FriendshipQueries.CreateFriendEntry(responder));
        }

        return isAccepted;
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
