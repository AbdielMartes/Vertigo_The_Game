using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;

const int MaximumMatchAttempts = 12;

try
{
    bool played = false;
    for (int attempt = 0; attempt < MaximumMatchAttempts && !played; attempt++)
    {
        played = await RunMatchAsync();
    }

    Require(played, "No legal opening placement found in the allotted match attempts.");
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

static async Task<bool> RunMatchAsync()
{
    using ServiceProvider host = CreateClient();
    using ServiceProvider member = CreateClient();
    using ServiceProvider outsider = CreateClient();
    using ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
    using ILobbyClient memberLobby = member.GetRequiredService<ILobbyClient>();
    using ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();

    try
    {
        UserSession hostSession = CreateGuest(host);
        UserSession memberSession = CreateGuest(member);
        string hostToken = hostSession.Token;
        string memberToken = memberSession.Token;
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

        (int CardNumber, int PilePosition) move = FindPlacement(first, hostHand);
        if (move.PilePosition == 0)
        {
            return false;
        }

        await VerifyPlacementAsync(
            (hostLobby, hostSession), (memberLobby, memberSession), (lobbyCode, first, hostHand, move));

        string outsiderToken = CreateGuest(outsider).Token;
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
        return true;
    }
    finally
    {
        hostLobby.Disconnect();
        memberLobby.Disconnect();
        host.GetRequiredService<ISessionCoordinator>().Logout();
        member.GetRequiredService<ISessionCoordinator>().Logout();
        outsider.GetRequiredService<ISessionCoordinator>().Logout();
    }
}

static (int CardNumber, int PilePosition) FindPlacement(MatchBoardSnapshot board, MatchPrivateHand hand)
{
    const int MaximumCardDifference = 10;
    var placements = from card in hand.Cards
                     from pile in board.Piles
                     where pile.TopCardNumber is int top && card - top is >= 1 and <= MaximumCardDifference
                     select (card, pile.Position);

    return placements.FirstOrDefault();
}

static async Task VerifyPlacementAsync(
    (ILobbyClient Lobby, UserSession Session) host,
    (ILobbyClient Lobby, UserSession Session) member,
    (string LobbyCode, MatchBoardSnapshot Board, MatchPrivateHand Hand,
        (int CardNumber, int PilePosition) Move) opening)
{
    MatchBoardSnapshot initial = opening.Board;
    TaskCompletionSource<MatchBoardChanged> hostUpdate = new(TaskCreationOptions.RunContinuationsAsynchronously);
    TaskCompletionSource<MatchBoardChanged> memberUpdate = new(TaskCreationOptions.RunContinuationsAsynchronously);
    host.Lobby.MatchBoardUpdated += RecordHostUpdate;
    member.Lobby.MatchBoardUpdated += RecordMemberUpdate;

    try
    {
        MatchPlaceRequest request = new()
        {
            LobbyCode = opening.LobbyCode,
            SessionToken = host.Session.Token,
            CardNumber = opening.Move.CardNumber,
            PilePosition = opening.Move.PilePosition,
            ExpectedRevision = initial.Revision
        };
        MatchBoardSnapshot placed = host.Lobby.PlaceMatchCard(request);
        MatchBoardChanged[] changes = await Task.WhenAll(hostUpdate.Task, memberUpdate.Task)
            .WaitAsync(TimeSpan.FromSeconds(15));

        Require(placed.Revision == initial.Revision + 1 && changes.All(change =>
            change.LobbyCode == opening.LobbyCode && change.Revision == placed.Revision
            && change.PlayerUsername == host.Session.Username
            && change.CardNumber == request.CardNumber && change.PilePosition == request.PilePosition),
            "Placement did not notify both clients of the expected move.");

        MatchBoardSnapshot hostBoard = host.Lobby.GetMatchBoard(request.LobbyCode, host.Session.Token);
        MatchBoardSnapshot memberBoard = member.Lobby.GetMatchBoard(request.LobbyCode, member.Session.Token);
        foreach (MatchBoardSnapshot board in new[] { placed, hostBoard, memberBoard })
        {
            Require(board.MatchId == initial.MatchId && board.Revision == placed.Revision
                && board.CurrentTurnPlayerId == member.Session.PlayerId
                && board.Piles.Single(pile => pile.Position == request.PilePosition).TopCardNumber
                    == request.CardNumber,
                "A client received an incorrect board after the move.");
            Require(board.Players.Count == 2
                && board.Players.Any(player => player.PlayerId == host.Session.PlayerId
                    && player.Username == host.Session.Username)
                && board.Players.Any(player => player.PlayerId == member.Session.PlayerId
                    && player.Username == member.Session.Username),
                "A board snapshot did not preserve both player names.");
        }

        MatchPrivateHand hand = host.Lobby.GetMatchPrivateHand(request.LobbyCode, host.Session.Token);
        Require(hand.MatchId == initial.MatchId && hand.PlayerId == host.Session.PlayerId
            && hand.Cards.SequenceEqual(opening.Hand.Cards.Where(card => card != request.CardNumber)),
            "The host's hand did not lose exactly the played card.");
        Console.WriteLine("PASS: VALID PLACEMENT, BOARD NOTIFICATIONS, PLAYER NAMES AND PRIVATE HAND");
    }
    finally
    {
        host.Lobby.MatchBoardUpdated -= RecordHostUpdate;
        member.Lobby.MatchBoardUpdated -= RecordMemberUpdate;
    }

    void RecordHostUpdate(object? sender, MatchBoardChanged change)
    {
        if (change.MatchId == initial.MatchId)
        {
            hostUpdate.TrySetResult(change);
        }
    }

    void RecordMemberUpdate(object? sender, MatchBoardChanged change)
    {
        if (change.MatchId == initial.MatchId)
        {
            memberUpdate.TrySetResult(change);
        }
    }
}

static ServiceProvider CreateClient()
{
    ServiceCollection services = new();
    services.AddVertigoClientNetworking();
    return services.BuildServiceProvider();
}

static UserSession CreateGuest(ServiceProvider provider)
{
    var result = provider.GetRequiredService<ISessionCoordinator>().CreateGuestSession();
    if (!result.IsSuccess || result.Value is null)
    {
        throw new InvalidOperationException("Could not create test guest session.");
    }

    return result.Value;
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
