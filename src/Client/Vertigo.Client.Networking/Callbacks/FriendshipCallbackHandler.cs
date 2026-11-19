using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class FriendshipCallbackHandler : IFriendshipCallback
{
    public event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    public event EventHandler<FriendEntry>? FriendshipEstablished;

    public event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    public event EventHandler<string>? LobbyInvitationWithdrawn;

    void IFriendshipCallback.Notify(FriendshipNotification notification)
    {
        switch (notification)
        {
            case { Kind: FriendshipNotificationKind.FriendRequestReceived, FriendRequest: { } request,
                Friend: null, LobbyInvitation: null, WithdrawnLobbyCode: null }:
                FriendRequestReceived?.Invoke(this, request);
                break;

            case { Kind: FriendshipNotificationKind.FriendshipEstablished, Friend: { } friend,
                FriendRequest: null, LobbyInvitation: null, WithdrawnLobbyCode: null }:
                FriendshipEstablished?.Invoke(this, friend);
                break;

            case { Kind: FriendshipNotificationKind.LobbyInvitationReceived, LobbyInvitation: { } invitation,
                FriendRequest: null, Friend: null, WithdrawnLobbyCode: null }:
                LobbyInvitationReceived?.Invoke(this, invitation);
                break;

            case { Kind: FriendshipNotificationKind.LobbyInvitationWithdrawn, WithdrawnLobbyCode: { } lobbyCode,
                FriendRequest: null, Friend: null, LobbyInvitation: null }
                when !string.IsNullOrWhiteSpace(lobbyCode):
                LobbyInvitationWithdrawn?.Invoke(this, lobbyCode);
                break;
        }
    }
}
