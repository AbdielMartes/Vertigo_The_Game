using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.Networking.Sessions.Services;

public sealed class SessionTransportCleaner : ISessionTransportCleaner
{
    private readonly ILobbyClient _lobby;
    private readonly IFriendshipNotificationClient _notifications;

    public SessionTransportCleaner(ILobbyClient lobby, IFriendshipNotificationClient notifications)
    {
        _lobby = lobby;
        _notifications = notifications;
    }

    public void DisconnectAll()
    {
        _lobby.Disconnect();
        _notifications.Disconnect();
    }
}
