using Vertigo.Server.PlayerManagement.Friendships.Models;
using Vertigo.Contracts.Internal;
using System.Collections.Concurrent;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

public sealed class FriendshipNotifier : IFriendshipNotifier
{
    private readonly ConcurrentDictionary<Guid, FriendshipSubscription> _subscriptions = new();

    public void Subscribe(Guid playerId, IFriendshipCallback callback, string sessionId)
    {
        _subscriptions[playerId] = new FriendshipSubscription(callback, sessionId);
    }

    public void Unsubscribe(Guid playerId, string sessionId)
    {
        if (_subscriptions.TryGetValue(playerId, out FriendshipSubscription? subscription)
            && subscription.SessionId == sessionId)
        {
            ICollection<KeyValuePair<Guid, FriendshipSubscription>> entries = _subscriptions;
            entries.Remove(new KeyValuePair<Guid, FriendshipSubscription>(playerId, subscription));
        }
    }

    public void UnsubscribeSession(string sessionId)
    {
        List<Guid> playerIds = _subscriptions
            .Where(pair => pair.Value.SessionId == sessionId)
            .Select(pair => pair.Key)
            .ToList();

        foreach (Guid playerId in playerIds)
        {
            Unsubscribe(playerId, sessionId);
        }
    }

    public void NotifyFriendRequestReceived(Guid targetPlayerId, FriendshipInvitationEntry request)
    {
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.FriendRequestReceived,
            FriendRequest = request
        };

        Send(targetPlayerId, notification);
    }

    public void NotifyFriendshipEstablished(Guid targetPlayerId, FriendEntry friend)
    {
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.FriendshipEstablished,
            Friend = friend
        };

        Send(targetPlayerId, notification);
    }

    public void NotifyLobbyInvitationReceived(Guid targetPlayerId, LobbyInvitationEntry invitation)
    {
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.LobbyInvitationReceived,
            LobbyInvitation = invitation
        };

        Send(targetPlayerId, notification);
    }

    public void NotifyLobbyInvitationWithdrawn(Guid targetPlayerId, string lobbyCode)
    {
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.LobbyInvitationWithdrawn,
            WithdrawnLobbyCode = lobbyCode
        };

        Send(targetPlayerId, notification);
    }

    public bool IsOnline(Guid playerId)
    {
        bool isOnline = _subscriptions.ContainsKey(playerId);

        return isOnline;
    }

    private void Send(Guid targetPlayerId, FriendshipNotification notification)
    {
        if (_subscriptions.TryGetValue(targetPlayerId, out FriendshipSubscription? subscription))
        {
            try
            {
                subscription.Callback.Notify(notification);
            }
            catch (CommunicationException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (TimeoutException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
            catch (ObjectDisposedException)
            {
                Unsubscribe(targetPlayerId, subscription.SessionId);
            }
        }
    }
}
