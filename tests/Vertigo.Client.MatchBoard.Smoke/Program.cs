using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;

using ServiceProvider host = CreateClient();
using ServiceProvider member = CreateClient();
using ServiceProvider outsider = CreateClient();
using ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
using ILobbyClient memberLobby = member.GetRequiredService<ILobbyClient>();
using ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();

try
{
    string hostToken = CreateGuest(host);
    string memberToken = CreateGuest(member);
    string lobbyCode = hostLobby.CreateLobby(true, 2, hostToken);
    memberLobby.JoinLobby(lobbyCode, memberToken);
    hostLobby.SetReady(lobbyCode, true, hostToken);
    memberLobby.SetReady(lobbyCode, true, memberToken);
    LobbyPreparation started = hostLobby.StartMatch(lobbyCode, hostToken);
    Require(started.MatchId != Guid.Empty, "StartMatch returned an empty MatchId.");

    LobbyPreparation? membership = memberLobby.GetCurrentLobby(memberToken);
    Require(membership?.LobbyCode == lobbyCode && membership.MatchId == started.MatchId,
        "Current lobby does not match the running match.");

    MatchBoardSnapshot first = hostLobby.GetMatchBoard(lobbyCode, hostToken);
    MatchBoardSnapshot second = memberLobby.GetMatchBoard(lobbyCode, memberToken);
    Require(first.MatchId == started.MatchId, "Host board has wrong MatchId.");
    Require(second.MatchId == started.MatchId, "Member board has wrong MatchId.");
    Require(first.Players.Count == 2 && second.Players.Count == 2,
        "Match board did not return two participants.");
    Console.WriteLine("PASS: GET MATCH BOARD OVER NETTCP FOR BOTH PARTICIPANTS");

    MatchPrivateHand hostHand = hostLobby.GetMatchPrivateHand(lobbyCode, hostToken);
    MatchPrivateHand memberHand = memberLobby.GetMatchPrivateHand(lobbyCode, memberToken);
    Require(hostHand.MatchId == started.MatchId && memberHand.MatchId == started.MatchId,
        "Private hands have the wrong match identifier.");
    Require(hostHand.PlayerId != memberHand.PlayerId && hostHand.Cards.Count == 6 && memberHand.Cards.Count == 6
        && hostHand.Cards.Concat(memberHand.Cards).Distinct().Count() == 12, "Private hands overlap or are invalid.");

    bool wrongTurnDenied = false;
    try
    {
        memberLobby.PlaceMatchCard(new MatchPlaceRequest
        {
            LobbyCode = lobbyCode,
            SessionToken = memberToken,
            CardNumber = memberHand.Cards[0],
            PilePosition = 1,
            ExpectedRevision = second.Revision
        });
    }
    catch (FaultException<ServiceFault> exception)
    {
        wrongTurnDenied = exception.Detail.MessageKey == "Match.Error.NotYourTurn";
    }

    Require(wrongTurnDenied, "Placement outside the player's turn was not rejected.");
    Require(hostLobby.GetMatchBoard(lobbyCode, hostToken).Revision == first.Revision,
        "Rejected placement changed the board revision.");
    Console.WriteLine("PASS: WRONG TURN REJECTED WITHOUT CHANGING BOARD");

    string outsiderToken = CreateGuest(outsider);
    bool outsiderDenied = false;
    try
    {
        outsiderLobby.GetMatchPrivateHand(lobbyCode, outsiderToken);
    }
    catch (FaultException<ServiceFault> exception)
    {
        outsiderDenied = exception.Detail.ErrorCode == ServiceErrorCodes.LobbyNotMember;
    }

    Require(outsiderDenied, "An outsider was able to read the private hand.");
    memberLobby.Disconnect();
    memberLobby.JoinLobby(lobbyCode, memberToken);
    MatchPrivateHand resumedHand = memberLobby.GetMatchPrivateHand(lobbyCode, memberToken);
    Require(resumedHand.MatchId == memberHand.MatchId && resumedHand.PlayerId == memberHand.PlayerId
        && resumedHand.Cards.SequenceEqual(memberHand.Cards), "Private hand changed after reconnecting.");
    Console.WriteLine("PASS: PRIVATE HANDS, OUTSIDER DENIAL AND RECONNECTION");
    return 0;
}
catch (FaultException<ServiceFault> exception)
{
    Console.Error.WriteLine($"FAIL BUSINESS: {exception.Detail.ErrorCode}; Key={exception.Detail.MessageKey}");
    return 1;
}
catch (TimeoutException)
{
    Console.Error.WriteLine("FAIL TIMEOUT: GetMatchBoard smoke timed out.");
    return 1;
}
catch (CommunicationException exception)
{
    Console.Error.WriteLine($"FAIL TRANSPORT: {exception.GetType().Name}");
    return 1;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL UNEXPECTED: {exception.GetType().Name} (0x{exception.HResult:X8})");
    return 1;
}
finally
{
    hostLobby.Disconnect();
    memberLobby.Disconnect();
    host.GetRequiredService<ISessionCoordinator>().Logout();
    member.GetRequiredService<ISessionCoordinator>().Logout();
    outsider.GetRequiredService<ISessionCoordinator>().Logout();
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static string CreateGuest(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not create test guest session.");
    }

    return result.Value.Token;
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
