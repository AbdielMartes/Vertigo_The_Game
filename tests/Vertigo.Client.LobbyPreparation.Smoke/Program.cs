using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;

using ServiceProvider first = CreateClient();
using ServiceProvider second = CreateClient();
using ILobbyClient hostLobby = first.GetRequiredService<ILobbyClient>();
using ILobbyClient guestLobby = second.GetRequiredService<ILobbyClient>();
using ManualResetEventSlim hostReadyEvent = new(false);
using ManualResetEventSlim guestStartedEvent = new(false);
object matchSync = new();
Guid callbackMatchId = Guid.Empty;
long callbackRevision = -1;
using ManualResetEventSlim hostTransferredEvent = new(false);

string? firstCode = null;
string? secondCode = null;
int exitCode = 0;
try
{
    string hostToken = CreateGuest(first);
    string guestToken = CreateGuest(second);
    Guid hostId = first.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;
    Guid guestId = second.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;

    hostLobby.PreparationChanged += (_, preparation) =>
    {
        if (preparation.LobbyCode == firstCode && preparation.ReadyPlayerIds.Contains(guestId))
        {
            hostReadyEvent.Set();
        }
    };
    guestLobby.PreparationChanged += (_, preparation) =>
    {
        if (preparation.LobbyCode == firstCode && preparation.IsInGame)
        {
            lock (matchSync)
            {
                callbackMatchId = preparation.MatchId;
                callbackRevision = preparation.Revision;
            }
            guestStartedEvent.Set();
        }
        if (preparation.LobbyCode == secondCode && preparation.HostPlayerId == guestId)
        {
            hostTransferredEvent.Set();
        }
    };

    firstCode = hostLobby.CreateLobby(true, 2, hostToken);
    guestLobby.JoinLobby(firstCode, guestToken);
    bool notReadyDenied = false;
    try
    {
        hostLobby.StartMatch(firstCode, hostToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        notReadyDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotReady;
    }
    Require(notReadyDenied, "Host could start without all players being ready");
    Console.WriteLine("PASS: NOT-READY REJECTED");

    LobbyPreparation firstReady = hostLobby.SetReady(firstCode, true, hostToken);
    Require(firstReady.ReadyPlayerIds.Contains(hostId), "Host ready flag missing");
    LobbyPreparation secondReady = guestLobby.SetReady(firstCode, true, guestToken);
    Require(secondReady.ReadyPlayerIds.Contains(guestId), "Guest ready flag missing");
    Require(secondReady.Revision > firstReady.Revision, "Ready revisions did not advance");
    Require(hostReadyEvent.Wait(TimeSpan.FromSeconds(10)), "Host missed Ready callback");
    Console.WriteLine("PASS: READY CALLBACK");

    bool guestDenied = false;
    try
    {
        guestLobby.StartMatch(firstCode, guestToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        guestDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyNotHost;
    }
    Require(guestDenied, "Non-host unexpectedly started match");
    Console.WriteLine("PASS: NON-HOST REJECTED");

    LobbyPreparation started = hostLobby.StartMatch(firstCode, hostToken);
    Require(started.IsInGame, "StartMatch did not change the lobby state");
    Require(guestStartedEvent.Wait(TimeSpan.FromSeconds(10)), "Guest missed match-start callback");
    Require(started.MatchId != Guid.Empty, "StartMatch returned an empty MatchId");
    LobbyPreparation queried = guestLobby.GetPreparation(firstCode, guestToken);
    Require(queried.MatchId == started.MatchId, "RPC GetPreparation MatchId differs from StartMatch");
    Require(queried.Revision == started.Revision, "RPC revisions differ after match start");
    lock (matchSync)
    {
        Require(callbackMatchId == started.MatchId, "Callback MatchId differs from RPC results");
        Require(callbackRevision == started.Revision, "Callback revision differs from RPC results");
    }
    Console.WriteLine("PASS: MATCH ID CONSISTENT ACROSS RPC AND CALLBACK");

    MatchBoardSnapshot hostBoard = hostLobby.GetMatchBoard(firstCode, hostToken);
    MatchBoardSnapshot guestBoard = guestLobby.GetMatchBoard(firstCode, guestToken);
    Require(hostBoard.MatchId == started.MatchId && guestBoard.MatchId == started.MatchId,
        "Public board MatchId mismatch");
    foreach (MatchBoardSnapshot board in new[] { hostBoard, guestBoard })
    {
        Require(board.Players.Count == 2 && board.Players.All(player =>
            !string.IsNullOrWhiteSpace(player.Username) && player.FloorsBuilt == 0), "Invalid public participants");
        Require(board.RoundNumber == 1 && board.Players.Any(player => player.PlayerId == board.CurrentTurnPlayerId),
            "Initial round or turn did not match the server rules");
        Require(board.Piles.Select(pile => pile.Position).SequenceEqual(new[] { 1, 2, 3, 4 })
            && board.Piles.All(pile => pile.CardCount == 1 && pile.TopCardNumber is not null),
            "Initial towers did not contain one foundation each");
    }
    Require(hostBoard.CurrentTurnPlayerId == guestBoard.CurrentTurnPlayerId, "Clients disagree on the opening turn");
    Console.WriteLine("PASS: BOTH CLIENTS READ PUBLIC BOARD");

    bool repeatStartDenied = false;
    try
    {
        hostLobby.StartMatch(firstCode, hostToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        repeatStartDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyAlreadyInGame;
    }
    Require(repeatStartDenied, "Second StartMatch was not rejected as AlreadyInGame");
    LobbyPreparation afterRejectedStart = hostLobby.GetPreparation(firstCode, hostToken);
    Require(afterRejectedStart.MatchId == started.MatchId, "Rejected start changed MatchId");
    Require(afterRejectedStart.Revision == started.Revision, "Rejected start changed revision");
    Console.WriteLine("PASS: SECOND START REJECTED WITHOUT CHANGING MATCH");
    bool readyAfterStartDenied = false;
    try
    {
        guestLobby.SetReady(firstCode, false, guestToken);
    }
    catch (FaultException<ServiceFault> fault)
    {
        readyAfterStartDenied = fault.Detail.ErrorCode == ServiceErrorCodes.LobbyAlreadyInGame;
    }
    Require(readyAfterStartDenied, "SetReady succeeded after match start");
    Console.WriteLine("PASS: READY AFTER START REJECTED");
    Console.WriteLine("PASS: START CALLBACK");

    Require(!guestLobby.LeaveLobby(firstCode, guestToken), "Guest unexpectedly left an active match");
    Require(!hostLobby.LeaveLobby(firstCode, hostToken), "Host unexpectedly left an active match");
    guestLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(first);
    LogoutQuietly(second);

    hostToken = CreateGuest(first);
    guestToken = CreateGuest(second);
    guestId = second.GetRequiredService<PlayerSession>().GetActiveSession()!.PlayerId;

    secondCode = hostLobby.CreateLobby(true, 2, hostToken);
    hostLobby.SetReady(secondCode, true, hostToken);
    guestLobby.JoinLobby(secondCode, guestToken);
    LobbyPreparation afterJoining = guestLobby.GetPreparation(secondCode, guestToken);
    Require(afterJoining.ReadyPlayerIds.Count == 0, "New arrival did not reset Ready flags");
    Console.WriteLine("PASS: NEW PLAYER RESETS READY");
    Require(hostLobby.LeaveLobby(secondCode, hostToken), "Host could not leave second lobby");
    Require(hostTransferredEvent.Wait(TimeSpan.FromSeconds(10)), "Guest missed host-transfer callback");
    LobbyPreparation afterTransfer = guestLobby.GetPreparation(secondCode, guestToken);
    Require(afterTransfer.HostPlayerId == guestId, "Host was not reassigned");
    Console.WriteLine("PASS: HOST TRANSFER CALLBACK");
    Require(guestLobby.LeaveLobby(secondCode, guestToken), "New host could not leave");
    Console.WriteLine("PASS: Lobby preparation smoke");
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL: {exception.GetType().Name}: {exception.Message}");
    exitCode = 1;
}
finally
{
    guestLobby.Disconnect();
    hostLobby.Disconnect();
    LogoutQuietly(first);
    LogoutQuietly(second);
}

return exitCode;

static ServiceProvider CreateClient()
{
    ServiceCollection registrations = new();
    registrations.AddVertigoClientNetworking();
    return registrations.BuildServiceProvider();
}

static string CreateGuest(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not authenticate guest");
    }
    return result.Value.Token;
}

static void LogoutQuietly(ServiceProvider provider)
{
    try
    {
        provider.GetRequiredService<ISessionCoordinator>().Logout();
    }
    catch (CommunicationException)
    {
        // Cleanup is best-effort when the server has disconnected.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
