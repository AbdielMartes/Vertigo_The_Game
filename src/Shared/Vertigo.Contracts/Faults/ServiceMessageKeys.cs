namespace Vertigo.Contracts.Faults;

public static class ServiceMessageKeys
{
    public const string UsernameAlreadyRegistered = "RegisterAccount.UsernameAlreadyRegistered";
    public const string EmailAlreadyRegistered = "RegisterAccount.EmailAlreadyRegistered";
    public const string InvalidRegistrationData = "RegisterAccount.ValidationSummary";
    public const string LobbyNotFound = "ROOM_NOT_FOUND";
    public const string LobbyFull = "ROOM_FULL";    
    public const string LobbyAlreadyStarted = "MSG-ROOM-009";
    public const string InvalidLobbyCapacity = "MSG-ROOM-011";
    public const string InvalidData = "ErrorInvalidData";
    public const string Unexpected = "Error.Unknown";
    public const string UserNotFound = "ErrorUserNotFound";
    public const string InvalidCode = "ErrorInvalidCode";
    public const string InvalidCredentials = "ErrorInvalidCredentials";
    public const string SessionAlreadyActive = "AccessMenu.SessionAlreadyActive";
    public const string FriendRequestAlreadySent = "Friends.RequestSent";
    public const string AlreadyFriends = "Friends.AlreadyFriends";
    public const string FriendshipBlocked = "Friends.RequestBlocked";
    public const string FriendRequestNotFound = "Notifications.FriendRequestNotFound";
    public const string LobbyInviteNotAllowed = "InviteFriends.NotAllowed";
    public const string PlayerAlreadyInLobby = "Lobby.PlayerAlreadyInLobby";
    
    public const string LobbyNotMember = "Lobby.NotMember";
    public const string LobbyNotHost = "Lobby.NotHost";
    public const string LobbyNotReady = "Lobby.NotReady";
    public const string LobbyAlreadyInGame = "Lobby.AlreadyInGame";

    public const string Unauthorized = "Authentication.Unauthorized";
    public const string ChatLanguageBlocked = "Chat.LanguageBlocked";
    public const string LobbyChatNotAllowed = "Lobby.ChatNotAllowed";
}
