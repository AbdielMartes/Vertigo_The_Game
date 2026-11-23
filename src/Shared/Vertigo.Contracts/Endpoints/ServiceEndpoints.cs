namespace Vertigo.Contracts.Endpoints;

public static class ServiceEndpoints
{
    public const int TcpPort = 8081;

    public static readonly string AccountServiceAddress = $"net.tcp://localhost:{TcpPort}/AccountService";
    public static readonly string AuthServiceAddress = $"net.tcp://localhost:{TcpPort}/AuthService";
    public static readonly string FriendshipServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipService";
    public static readonly string FriendshipNotificationServiceAddress = $"net.tcp://localhost:{TcpPort}/FriendshipNotificationService";
    public static readonly string LobbyInvitationServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyInvitationService";
    public static readonly string BlockedPlayerServiceAddress = $"net.tcp://localhost:{TcpPort}/BlockedPlayerService";
    public static readonly string ChatServiceAddress = $"net.tcp://localhost:{TcpPort}/ChatService";
    public static readonly string LobbyServiceAddress = $"net.tcp://localhost:{TcpPort}/LobbyService";
    public static readonly string StatisticsServiceAddress = $"net.tcp://localhost:{TcpPort}/StatisticsService";
}
