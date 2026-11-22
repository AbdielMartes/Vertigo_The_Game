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

int exitCode = 2;
if (string.IsNullOrWhiteSpace(hostUsername) || string.IsNullOrWhiteSpace(hostPassword)
    || string.IsNullOrWhiteSpace(inviteeUsername) || string.IsNullOrWhiteSpace(inviteePassword))
{
    Console.Error.WriteLine("CONFIGURATION REQUIRED: set the four VERTIGO_TEST_* environment variables in Rider or your shell.");
}
else
{
    using ServiceProvider host = CreateClient();
    using ServiceProvider invitee = CreateClient();
    ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
    ILobbyClient inviteeLobby = invitee.GetRequiredService<ILobbyClient>();
    IFriendshipNotificationClient inviteeNotifications = invitee.GetRequiredService<IFriendshipNotificationClient>();
    string? hostToken = null;
    string? inviteeToken = null;
    string? liveLobbyCode = null;
    string? closedLobbyCode = null;
    string? disconnectedLobbyCode = null;
    TaskCompletionSource<LobbyInvitationEntry> invitationReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
    TaskCompletionSource<string> invitationWithdrawn = new(TaskCreationOptions.RunContinuationsAsynchronously);
    TaskCompletionSource<string> disconnectedWithdrawal = new(TaskCreationOptions.RunContinuationsAsynchronously);
    inviteeNotifications.LobbyInvitationReceived += ObserveInvitation;
    inviteeNotifications.LobbyInvitationWithdrawn += ObserveWithdrawal;

    exitCode = 0;
    try
    {
        UserSession hostSession = Login(host, hostUsername, hostPassword);
        UserSession inviteeSession = Login(invitee, inviteeUsername, inviteePassword);
        Require(hostSession.PlayerId != inviteeSession.PlayerId, "Use two different registered accounts.");
        hostToken = hostSession.Token;
        inviteeToken = inviteeSession.Token;

        RegisteredClient owner = new(host, hostSession);
        RegisteredClient invitedPlayer = new(invitee, inviteeSession);
        EnsureFriendship(owner, invitedPlayer);
        Require(inviteeNotifications.Subscribe(inviteeToken), "Invitee notification subscription was rejected.");
        Console.WriteLine("Registered accounts: AUTHENTICATED, FRIENDS, INVITEE ONLINE");

        // Positive scenario: the server must authorize admission to a PRIVATE room.
        liveLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
        ClientResult<List<FriendEntry>> eligible = host.GetRequiredService<ILobbyInvitationClient>()
            .GetInvitableFriends(liveLobbyCode, hostToken);
        Require(eligible.IsSuccess && eligible.Value?.Any(friend => friend.PlayerId == inviteeSession.PlayerId) == true,
            "The online friend was missing from the list of eligible invitees.");
        SendInvitation(owner, liveLobbyCode, inviteeSession.PlayerId);
        LobbyInvitationEntry notice = await invitationReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Require(notice.LobbyCode == liveLobbyCode, "Invitation callback named the wrong lobby.");
        Require(HasInvitation(invitee, liveLobbyCode, inviteeToken), "Valid invitation not visible to invitee.");
        LobbyAdmission admission = inviteeLobby.JoinLobby(liveLobbyCode, inviteeToken);
        Require(string.Equals(admission.LobbyCode, liveLobbyCode, StringComparison.OrdinalIgnoreCase),
            "Private lobby admission did not return the expected code.");
        Require(!HasInvitation(invitee, liveLobbyCode, inviteeToken),
            "Accepted invitation was still pending.");
        Console.WriteLine("PASS: valid private invitation accepted and consumed by server");

        Require(inviteeLobby.LeaveLobby(liveLobbyCode, inviteeToken), "Invitee failed to leave private room.");
        SendInvitation(owner, liveLobbyCode, inviteeSession.PlayerId);
        Require(HasInvitation(invitee, liveLobbyCode, inviteeToken), "The invitation to decline was not pending.");
        ClientResult<bool> declined = invitee.GetRequiredService<ILobbyInvitationClient>()
            .DeclineLobbyInvitation(liveLobbyCode, inviteeToken);
        Require(declined.IsSuccess && declined.Value && !HasInvitation(invitee, liveLobbyCode, inviteeToken),
            "Declining the invitation did not remove it from the pending list.");
        Console.WriteLine("PASS: eligible friend can decline an invitation");
        Require(hostLobby.LeaveLobby(liveLobbyCode, hostToken), "Host failed to leave private room.");
        liveLobbyCode = null;

        // Negative scenario: an already issued invitation must not admit the player
        // after the host closes the room, even if a stale UI item remains visible.
        closedLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
        SendInvitation(owner, closedLobbyCode, inviteeSession.PlayerId);
        Require(HasInvitation(invitee, closedLobbyCode, inviteeToken), "Second invitation was not created.");
        Require(hostLobby.LeaveLobby(closedLobbyCode, hostToken), "Host did not close second room.");
        string withdrawn = await invitationWithdrawn.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Require(withdrawn == closedLobbyCode, "Invitation withdrawal callback named the wrong lobby.");

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
        disconnectedLobbyCode = hostLobby.CreateLobby(false, 2, hostToken);
        SendInvitation(owner, disconnectedLobbyCode, inviteeSession.PlayerId);
        Require(HasInvitation(invitee, disconnectedLobbyCode, inviteeToken), "Disconnect test invitation was not pending.");
        hostLobby.Disconnect();

        string removedLobby = await disconnectedWithdrawal.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Require(removedLobby == disconnectedLobbyCode && !HasInvitation(invitee, removedLobby, inviteeToken),
            "Disconnecting the last member did not withdraw the invitation.");
        Require(hostLobby.GetCurrentLobby(hostToken) is null, "The disconnected host still belongs to the lobby.");
        Console.WriteLine("PASS: disconnecting the last member removes the lobby and withdraws its invitation");
        IBlockedPlayerClient blocks = host.GetRequiredService<IBlockedPlayerClient>();
        ClientResult<bool> blocked = blocks.BlockPlayer(inviteeSession.PlayerId, hostToken);
        Require(blocked.IsSuccess && blocked.Value, "Host could not block the test account.");
        try
        {
            Require(FindRelation(host.GetRequiredService<IFriendshipClient>(), hostSession, inviteeSession)
                == PlayerRelation.Blocked, "The blocker did not see the blocked relationship.");
            Require(FindRelation(invitee.GetRequiredService<IFriendshipClient>(), inviteeSession, hostSession) is null,
                "The blocked account could still find the player who blocked it.");
        }
        finally
        {
            ClientResult<bool> unblocked = blocks.UnblockPlayer(inviteeSession.PlayerId, hostToken);
            Require(unblocked.IsSuccess && unblocked.Value, "Host could not undo the test block.");
        }
        Console.WriteLine("PASS: player search preserves relationships and blocking direction");
        Console.WriteLine("PASS: Private invitations smoke");
    }
    catch (Exception exception)
    {
        // Never print session tokens or passwords.
        Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {SafeError(exception)}");
        exitCode = 1;
    }
    finally
    {
        inviteeNotifications.Disconnect();
        inviteeLobby.Disconnect();
        hostLobby.Disconnect();
        LogoutQuietly(host);
        LogoutQuietly(invitee);
    }

    void ObserveInvitation(object? sender, LobbyInvitationEntry invitation)
    {
        if (invitation.LobbyCode == liveLobbyCode)
        {
            invitationReceived.TrySetResult(invitation);
        }
    }

    void ObserveWithdrawal(object? sender, string lobbyCode)
    {
        if (lobbyCode == closedLobbyCode)
        {
            invitationWithdrawn.TrySetResult(lobbyCode);
        }
        else if (lobbyCode == disconnectedLobbyCode)
        {
            disconnectedWithdrawal.TrySetResult(lobbyCode);
        }
    }
}

return exitCode;

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

static void EnsureFriendship(RegisteredClient owner, RegisteredClient other)
{
    IFriendshipClient ownerFriends = owner.Provider.GetRequiredService<IFriendshipClient>();
    ClientResult<List<FriendEntry>> friends = ownerFriends.GetFriends(owner.Session.Token);
    Require(friends.IsSuccess && friends.Value is not null, "Could not read host friendships.");
    bool areFriends = friends.Value!.Any(friend => friend.PlayerId == other.Session.PlayerId);
    if (!areFriends)
    {
        ClientResult<PlayerRelation> request = ownerFriends.SendFriendRequest(other.Session.PlayerId, owner.Session.Token);
        Require(request.IsSuccess, $"Could not send test friend request ({request.ErrorCode}).");
        if (request.Value != PlayerRelation.Friends)
        {
            Require(FindRelation(ownerFriends, owner.Session, other.Session) == PlayerRelation.RequestSent,
                "Player search did not show the outgoing pending friendship request.");
            IFriendshipClient inviteeFriends = other.Provider.GetRequiredService<IFriendshipClient>();
            ClientResult<PlayerRelation> duplicate = ownerFriends.SendFriendRequest(other.Session.PlayerId, owner.Session.Token);
            Require(duplicate.ErrorCode == ServiceErrorCodes.FriendRequestAlreadySent,
                "A duplicate friendship request did not return the expected fault.");
            Require(CountPendingRequestsFrom(inviteeFriends, other.Session, owner.Session) == 1,
                "The recipient did not see exactly one pending request.");
            ClientResult<bool> response = inviteeFriends.RespondToFriendship(owner.Session.PlayerId, true, other.Session.Token);
            Require(response.IsSuccess && response.Value, $"Could not accept test friend request ({response.ErrorCode}).");
            Require(CountPendingRequestsFrom(inviteeFriends, other.Session, owner.Session) == 0,
                "The accepted friendship still appeared as pending.");
        }
    }
    friends = ownerFriends.GetFriends(owner.Session.Token);
    Require(friends.IsSuccess && friends.Value?.Any(friend => friend.PlayerId == other.Session.PlayerId) == true,
        "Test accounts are not friends.");
    Require(FindRelation(ownerFriends, owner.Session, other.Session) == PlayerRelation.Friends,
        "Player search did not show the accepted friendship.");
    ClientResult<PlayerRelation> existing = ownerFriends.SendFriendRequest(other.Session.PlayerId, owner.Session.Token);
    Require(existing.ErrorCode == ServiceErrorCodes.AlreadyFriends,
        "An existing friendship did not prevent a new request.");
}

static int CountPendingRequestsFrom(IFriendshipClient client, UserSession recipient, UserSession sender)
{
    ClientResult<List<FriendshipInvitationEntry>> result = client.GetPendingFriendRequests(recipient.Token);
    Require(result.IsSuccess && result.Value is not null, "Could not read pending friendship requests.");
    return result.Value!.Count(request => request.SenderId == sender.PlayerId);
}

static PlayerRelation? FindRelation(IFriendshipClient client, UserSession viewer, UserSession target)
{
    ClientResult<List<PlayerSearchEntry>> result = client.SearchPlayers(target.Username, viewer.Token);
    Require(result.IsSuccess && result.Value is not null, "Player search failed.");
    return result.Value!.SingleOrDefault(entry => entry.PlayerId == target.PlayerId)?.Relation;
}

static void SendInvitation(RegisteredClient owner, string code, Guid inviteeId)
{
    ILobbyInvitationClient invitations = owner.Provider.GetRequiredService<ILobbyInvitationClient>();
    ClientResult<List<Guid>> result = invitations.SendLobbyInvitations(code, new List<Guid> { inviteeId }, owner.Session.Token);
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

internal sealed record RegisteredClient(ServiceProvider Provider, UserSession Session);
