using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IFriendshipNotificationClient : IDisposable
{
    event EventHandler<FriendshipInvitationEntry>? FriendRequestReceived;

    event EventHandler<FriendEntry>? FriendshipEstablished;

    event EventHandler<LobbyInvitationEntry>? LobbyInvitationReceived;

    event EventHandler<string>? LobbyInvitationWithdrawn;

    bool Subscribe(string sessionToken);

    void Disconnect();

    void Unsubscribe();
}
