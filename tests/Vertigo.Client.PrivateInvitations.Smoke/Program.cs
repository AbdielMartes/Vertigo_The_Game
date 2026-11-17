using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

const string HostUsernameVariable = "VERTIGO_TEST_HOST_USER";
const string HostPasswordVariable = "VERTIGO_TEST_HOST_PASSWORD";
const string GuestUsernameVariable = "VERTIGO_TEST_INVITEE_USER";
const string GuestPasswordVariable = "VERTIGO_TEST_INVITEE_PASSWORD";

string? hostUsername = Environment.GetEnvironmentVariable(HostUsernameVariable);
string? hostPassword = Environment.GetEnvironmentVariable(HostPasswordVariable);
string? inviteeUsername = Environment.GetEnvironmentVariable(GuestUsernameVariable);
string? inviteePassword = Environment.GetEnvironmentVariable(GuestPasswordVariable);

if (string.IsNullOrWhiteSpace(hostUsername) || string.IsNullOrWhiteSpace(hostPassword)
    || string.IsNullOrWhiteSpace(inviteeUsername) || string.IsNullOrWhiteSpace(inviteePassword))
{
    Console.Error.WriteLine("CONFIGURATION REQUIRED: set the four VERTIGO_TEST_* environment variables in Rider or your shell.");
    return 2;
}

using ServiceProvider host = CreateClient();
using ServiceProvider invitee = CreateClient();
ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
ILobbyClient inviteeLobby = invitee.GetRequiredService<ILobbyClient>();
IFriendshipNotificationClient inviteeNotifications = invitee.GetRequiredService<IFriendshipNotificationClient>();
string? hostToken = null;
string? inviteeToken = null;
string? liveLobbyCode = null;
string? closedLobbyCode = null;

try
{
    UserSession hostSession = Login(host, hostUsername, hostPassword);
    UserSession inviteeSession = Login(invitee, inviteeUsername, inviteePassword);
    Require(hostSession.PlayerId != inviteeSession.PlayerId, "Use two different registered accounts.");
    hostToken = hostSession.Token;
    inviteeToken = inviteeSession.Token;

    EnsureFriendship(host, invitee, hostSession, inviteeSession);
    Require(inviteeNotifications.Subscribe(inviteeToken), "Invitee notification subscription was rejected.");
    Console.WriteLine("Registered accounts: AUTHENTICATED, FRIENDS, INVITEE ONLINE");

    // Positive scenario: the server must authorize admission to a PRIVATE room.
    liveLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
    SendInvitation(host, liveLobbyCode, inviteeSession.PlayerId, hostToken);
    Require(HasInvitation(invitee, liveLobbyCode, inviteeToken), "Valid invitation not visible to invitee.");
    LobbyAdmission admission = inviteeLobby.JoinLobby(liveLobbyCode, inviteeToken);
    Require(string.Equals(admission.LobbyCode, liveLobbyCode, StringComparison.OrdinalIgnoreCase),
        "Private lobby admission did not return the expected code.");
    Require(!HasInvitation(invitee, liveLobbyCode, inviteeToken),
        "Accepted invitation was still pending.");
    Console.WriteLine("PASS: valid private invitation accepted and consumed by server");

    Require(inviteeLobby.LeaveLobby(liveLobbyCode, inviteeToken), "Invitee failed to leave private room.");
    Require(hostLobby.LeaveLobby(liveLobbyCode, hostToken), "Host failed to leave private room.");
    liveLobbyCode = null;

    // Negative scenario: an already issued invitation must not admit the player
    // after the host closes the room, even if a stale UI item remains visible.
    closedLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
    SendInvitation(host, closedLobbyCode, inviteeSession.PlayerId, hostToken);
    Require(HasInvitation(invitee, closedLobbyCode, inviteeToken), "Second invitation was not created.");
    Require(hostLobby.LeaveLobby(closedLobbyCode, hostToken), "Host did not close second room.");

    bool rejected = false;
    try
    {
        inviteeLobby.JoinLobby(closedLobbyCode, inviteeToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        Require(fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotFound,
            $"Expected LobbyNotFound; received '{fault.Detail.ErrorCode}'.");
        string playerMessage = new ClientErrorText(new TextLocalizer())
            .FromException(fault, "Social.InvitationUnavailable");
        Require(!string.IsNullOrWhiteSpace(playerMessage)
            && !string.Equals(playerMessage, fault.Detail.MessageKey, StringComparison.Ordinal),
            "The server error was not converted into localized player text.");
        rejected = true;
    }
    Require(rejected, "Server admitted the invitee to a closed private room.");
    Console.WriteLine("PASS: closed private room rejected admission with localized error");
    Console.WriteLine("PASS: Private invitations smoke");
    return 0;
}
catch (Exception exception)
{
    // Never print session tokens or passwords.
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {SafeError(exception)}");
    return 1;
}
finally
{
    inviteeNotifications.Disconnect();
    inviteeLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(host);
    LogoutQuietly(invitee);
}

static string SafeError(Exception exception)
{
    // Avoid copying transport exceptions that could contain sensitive request details.
    return exception is FaultException<ServiceFault> fault
        ? $"Server business error {fault.Detail.ErrorCode}"
        : exception is CommunicationException or TimeoutException
            ? "CoreWCF connection or timeout failure"
            : exception.Message;
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static UserSession Login(ServiceProvider provider, string username, string password)
{
    ClientResult<UserSession> result = provider.GetRequiredService<ISessionCoordinator>()
        .Login(username, password);
    Require(result.IsSuccess && result.Value is not null && !result.Value.IsGuest,
        $"Could not authenticate registered test account '{username}'. Failure: {result.Failure} ({result.ErrorCode}).");
    return result.Value!;
}

static void EnsureFriendship(ServiceProvider host, ServiceProvider invitee, UserSession owner, UserSession other)
{
    IFriendshipClient ownerFriends = host.GetRequiredService<IFriendshipClient>();
    ClientResult<List<FriendEntry>> friends = ownerFriends.GetFriends(owner.Token);
    Require(friends.IsSuccess && friends.Value is not null, "Could not read host friendships.");
    bool areFriends = friends.Value!.Any(friend => friend.PlayerId == other.PlayerId);
    if (!areFriends)
    {
        ClientResult<PlayerRelation> request = ownerFriends.SendFriendRequest(other.PlayerId, owner.Token);
        Require(request.IsSuccess, $"Could not send test friend request ({request.ErrorCode}).");
        if (request.Value != PlayerRelation.Friends)
        {
            IFriendshipClient inviteeFriends = invitee.GetRequiredService<IFriendshipClient>();
            ClientResult<bool> response = inviteeFriends.RespondToFriendship(owner.PlayerId, true, other.Token);
            Require(response.IsSuccess && response.Value, $"Could not accept test friend request ({response.ErrorCode}).");
        }
    }
    friends = ownerFriends.GetFriends(owner.Token);
    Require(friends.IsSuccess && friends.Value?.Any(friend => friend.PlayerId == other.PlayerId) == true,
        "Test accounts are not friends.");
}

static void SendInvitation(ServiceProvider host, string code, Guid inviteeId, string hostToken)
{
    ILobbyInvitationClient invitations = host.GetRequiredService<ILobbyInvitationClient>();
    ClientResult<List<Guid>> result = invitations.SendLobbyInvitations(code, new List<Guid> { inviteeId }, hostToken);
    Require(result.IsSuccess && result.Value?.Contains(inviteeId) == true,
        $"Invitation was not sent. Failure={result.Failure}; ErrorCode={result.ErrorCode}. Check invitee subscription.");
}

static bool HasInvitation(ServiceProvider invitee, string code, string inviteeToken)
{
    ClientResult<List<LobbyInvitationEntry>> result = invitee.GetRequiredService<ILobbyInvitationClient>()
        .GetPendingLobbyInvitations(inviteeToken);
    Require(result.IsSuccess && result.Value is not null, "Could not query pending invitations.");
    return result.Value!.Any(entry => string.Equals(entry.LobbyCode, code, StringComparison.OrdinalIgnoreCase));
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Connection may already be gone during best-effort cleanup.
    }
    catch (TimeoutException)
    {
        // Connection may already be gone during best-effort cleanup.
    }
}

static void Require(bool condition, string reason)
{
    if (!condition)
    {
        throw new InvalidOperationException(reason);
    }
}
