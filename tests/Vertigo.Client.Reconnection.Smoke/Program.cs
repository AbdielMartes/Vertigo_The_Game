using System.Diagnostics;
using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

// Integration smoke: requires a running Vertigo.Server.Host with Fase 7B + Codex fixes.
// Each ServiceProvider has its own WCF duplex client and its own guest identity.
using ServiceProvider host = CreateClient();
using ServiceProvider guest = CreateClient();
using ServiceProvider outsider = CreateClient();
using ServiceProvider hostReconnect = CreateClient();
using ServiceProvider guestReconnect = CreateClient();
using ServiceProvider suspensionObserver = CreateClient();

ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
ILobbyClient guestLobby = guest.GetRequiredService<ILobbyClient>();
ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();
ILobbyClient resumedHostLobby = hostReconnect.GetRequiredService<ILobbyClient>();
ILobbyClient resumedGuestLobby = guestReconnect.GetRequiredService<ILobbyClient>();
ILobbyClient observerLobby = suspensionObserver.GetRequiredService<ILobbyClient>();
string? code = null;

try
{
    string hostToken = CreateGuest(host);
    string guestToken = CreateGuest(guest);
    string outsiderToken = CreateGuest(outsider);
    Guid hostId = host.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;
    Guid guestId = guest.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;

    code = hostLobby.CreateLobby(true, 2, hostToken);
    LobbyCredentials hostCredentials = new(code, hostToken);
    guestLobby.JoinLobby(code, guestToken);
    hostLobby.SetReady(code, true, hostToken);
    guestLobby.SetReady(code, true, guestToken);
    LobbyPreparation started = hostLobby.StartMatch(code, hostToken);
    Require(started.IsInGame && started.MatchId != Guid.Empty, "Match was not initialized");
    Guid matchId = started.MatchId;
    long revision = started.Revision;
    Console.WriteLine("PASS: MATCH STARTED WITH STABLE ID");

    // An unrelated account cannot enter an in-progress match.
    RequireFault(() => outsiderLobby.JoinLobby(code, outsiderToken), ServiceErrorCodes.LobbyAlreadyStarted);
    Console.WriteLine("PASS: THIRD PLAYER REJECTED");

    // Voluntary exit is not yet supported in active games; neither participant should be removed.
    Require(!guestLobby.LeaveLobby(code, guestToken), "LeaveLobby was accepted during a match");
    Require(guestLobby.GetPreparation(code, guestToken).MatchId == matchId,
        "Voluntary exit changed the match identifier");
    Console.WriteLine("PASS: IN-GAME LEAVE REJECTED");

    // Drop one duplex channel; keep the other online to observe the server-side change.
    guestLobby.Disconnect();
    LobbyPreparation afterGuestLoss = AwaitPreparation(hostLobby, hostCredentials,
        state => state.DisconnectedPlayerIds.Contains(guestId));
    Require(!afterGuestLoss.IsSuspended, "Game suspended with host still connected");
    Require(afterGuestLoss.MatchId == matchId && afterGuestLoss.Revision > revision,
        "Guest disconnect changed MatchId or failed to advance revision");
    revision = afterGuestLoss.Revision;
    Console.WriteLine("PASS: SINGLE DISCONNECT KEEPS MATCH RUNNING");

    // Reconnection is scoped to the same match and identity.
    MatchPresenceRequest reconnect = new() { MatchId = Guid.NewGuid(), LobbyCode = code, SessionToken = guestToken };
    RequireFault(() => resumedGuestLobby.ReconnectMatch(reconnect), ServiceErrorCodes.LobbyNotFound);
    Require(hostLobby.GetPreparation(code, hostToken).DisconnectedPlayerIds.Contains(guestId),
        "A mismatched match identifier restored the guest seat");
    reconnect.MatchId = matchId;
    resumedGuestLobby.ReconnectMatch(reconnect);
    MatchBoardSnapshot beforeConfirmation = resumedGuestLobby.GetMatchBoard(code, guestToken);
    MatchBoardSnapshot afterConfirmation = resumedGuestLobby.ConfirmMatchPresence(reconnect);
    Require(afterConfirmation.MatchId == matchId && afterConfirmation.Revision == beforeConfirmation.Revision,
        "Confirming presence without an active grace period changed the board");
    LobbyPreparation afterGuestReturn = resumedGuestLobby.GetPreparation(code, guestToken);
    Require(!afterGuestReturn.DisconnectedPlayerIds.Contains(guestId), "Guest still marked disconnected");
    Require(afterGuestReturn.MatchId == matchId && afterGuestReturn.Revision > revision,
        "Guest reconnect changed MatchId or failed to advance revision");
    revision = afterGuestReturn.Revision;
    Console.WriteLine("PASS: NEW GUEST CHANNEL RECONNECTS");

    // Replace a channel while the old one remains alive. Its late Closed callback must be ignored.
    resumedHostLobby.JoinLobby(code, hostToken);
    LobbyPreparation replaced = resumedHostLobby.GetPreparation(code, hostToken);
    Require(replaced.Revision > revision, "Host replacement did not advance revision");
    revision = replaced.Revision;
    hostLobby.Disconnect();
    // Allow the server to process Closed from the superseded channel, then force an RPC barrier.
    Thread.Sleep(SmokeTiming.StaleChannelCloseGrace);
    LobbyPreparation afterOldClose = resumedHostLobby.GetPreparation(code, hostToken);
    Require(!afterOldClose.DisconnectedPlayerIds.Contains(hostId),
        "Closing the old channel disconnected the replacement");
    Require(afterOldClose.MatchId == matchId, "Old channel close changed MatchId");
    Console.WriteLine("PASS: STALE CHANNEL CLOSE IGNORED");

    // Close all live match channels. After both closures, rejoin with another new channel.
    resumedGuestLobby.Disconnect();
    LobbyPreparation afterSecondGuestLoss = AwaitPreparation(resumedHostLobby, hostCredentials,
        state => state.DisconnectedPlayerIds.Contains(guestId));
    Require(!afterSecondGuestLoss.IsSuspended, "Game suspended with host connected");
    revision = afterSecondGuestLoss.Revision;
    resumedHostLobby.Disconnect();

    // A fresh, independent channel observes Game's suspended state without rejoining.
    // Poll rather than sleeping: the server may process Closed callbacks asynchronously.
    LobbyPreparation suspended = AwaitPreparation(observerLobby, hostCredentials,
        state => state.IsSuspended
            && state.DisconnectedPlayerIds.Contains(hostId)
            && state.DisconnectedPlayerIds.Contains(guestId));
    Require(suspended.IsInGame && suspended.MatchId == matchId,
        "Suspension must preserve the active match identifier");
    Require(suspended.Revision > revision,
        "Suspension did not advance the lobby revision");
    revision = suspended.Revision;
    Console.WriteLine("PASS: BOTH DISCONNECTED AND MATCH SUSPENDED");

    hostLobby.JoinLobby(code, hostToken); // Disconnected LobbyClient creates a fresh duplex channel.
    LobbyPreparation afterAllReturn = hostLobby.GetPreparation(code, hostToken);
    Require(afterAllReturn.MatchId == matchId, "Reconnection created a different match");
    Require(afterAllReturn.IsInGame && !afterAllReturn.IsSuspended,
        "Game did not resume Running after a participant returned");
    Require(afterAllReturn.DisconnectedPlayerIds.Contains(guestId),
        "Guest was unexpectedly reconnected when only host returned");
    Require(afterAllReturn.Revision > revision, "All-disconnected reconnect did not advance revision");
    Console.WriteLine("PASS: MATCH RECOVERED AFTER BOTH CHANNELS CLOSED");

    resumedGuestLobby.JoinLobby(code, guestToken);
    LobbyPreparation restored = resumedGuestLobby.GetPreparation(code, guestToken);
    Require(restored.DisconnectedPlayerIds.Count == 0, "Participants were not fully restored");
    Require(restored.MatchId == matchId && !restored.IsSuspended, "Final restoration changed match state");
    Console.WriteLine("PASS: BOTH PARTICIPANTS RESTORED");
    Console.WriteLine("PASS: Reconnection smoke");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    return 1;
}
finally
{
    // Match leave is intentionally rejected during InGame: close transports, not membership.
    hostLobby.Disconnect();
    guestLobby.Disconnect();
    outsiderLobby.Disconnect();
    resumedHostLobby.Disconnect();
    resumedGuestLobby.Disconnect();
    observerLobby.Disconnect();
    LogoutQuietly(host);
    LogoutQuietly(guest);
    LogoutQuietly(outsider);
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static string CreateGuest(ServiceProvider services)
{
    var result = services.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Guest authentication failed");
    }

    return result.Value.Token;
}

static LobbyPreparation AwaitPreparation(
    ILobbyClient lobby, LobbyCredentials credentials, Func<LobbyPreparation, bool> predicate)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    LobbyPreparation state = lobby.GetPreparation(credentials.Code, credentials.Token);
    while (!predicate(state) && stopwatch.Elapsed < SmokeTiming.ObservationTimeout)
    {
        Thread.Sleep(SmokeTiming.PollInterval);
        state = lobby.GetPreparation(credentials.Code, credentials.Token);
    }

    Require(predicate(state),
        $"Timed out after {SmokeTiming.ObservationTimeout.TotalSeconds}s waiting for expected NetTCP state");
    return state;
}

static void RequireFault(Action operation, string code)
{
    bool rejected = false;
    try
    {
        operation();
    }
    catch (FaultException<ServiceFault> fault)
    {
        rejected = fault.Detail.ErrorCode == code;
    }

    Require(rejected, $"Expected business fault {code}");
}

static void LogoutQuietly(ServiceProvider services)
{
    try
    {
        services.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Best-effort cleanup after an intentional duplex disconnect.
    }
    catch (TimeoutException)
    {
        // Best-effort cleanup after an intentional duplex disconnect.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

internal sealed record LobbyCredentials(string Code, string Token);

internal static class SmokeTiming
{
    internal static readonly TimeSpan ObservationTimeout = TimeSpan.FromSeconds(10);
    internal static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(100);
    internal static readonly TimeSpan StaleChannelCloseGrace = TimeSpan.FromMilliseconds(500);
}
