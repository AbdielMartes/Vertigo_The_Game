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

    Require(played, "No attempt exercised both an opening placement and a dismantle before round two.");
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
        Require(hostHand.PlayerId == hostSession.PlayerId && memberHand.PlayerId == memberSession.PlayerId
            && hostHand.Cards.Count == 10 && memberHand.Cards.Count == 10
            && hostHand.Cards.Concat(memberHand.Cards).Distinct().Count() == 20,
            "Private hands overlap or are invalid.");
        Require(first.CurrentTurnPlayerId == second.CurrentTurnPlayerId
            && first.Players.Any(player => player.PlayerId == first.CurrentTurnPlayerId),
            "Clients disagree on the opening turn or identify a non-participant.");

        bool isHostOpening = first.CurrentTurnPlayerId == hostSession.PlayerId;
        (ILobbyClient Lobby, UserSession Session, MatchPrivateHand Hand) opening = isHostOpening
            ? (hostLobby, hostSession, hostHand)
            : (memberLobby, memberSession, memberHand);
        (ILobbyClient Lobby, UserSession Session, MatchPrivateHand Hand) waiting = isHostOpening
            ? (memberLobby, memberSession, memberHand)
            : (hostLobby, hostSession, hostHand);

        bool wrongTurnDenied = false;
        try
        {
            waiting.Lobby.PlaceMatchCard(new MatchPlaceRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = waiting.Session.Token,
                CardNumber = waiting.Hand.Cards[0],
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

        bool planRequired = false;
        try
        {
            opening.Lobby.UseMatchMasterPlan(new MatchMasterPlanRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = opening.Session.Token,
                CardNumbers = opening.Hand.Cards.Take(2).ToList(),
                PilePosition = 1,
                ExpectedRevision = first.Revision
            });
        }
        catch (FaultException<ServiceFault> exception)
        {
            planRequired = exception.Detail.ErrorCode == "MATCH_MASTER_PLAN_REQUIRED"
                && exception.Detail.MessageKey == "Match.Error.MasterPlanRequired";
        }

        Require(planRequired, "Using a master plan without earning one did not return the expected fault.");
        MatchBoardSnapshot afterRejectedPlan = opening.Lobby.GetMatchBoard(lobbyCode, opening.Session.Token);
        Require(afterRejectedPlan.Revision == first.Revision && afterRejectedPlan.RecentMoves.Count == 0,
            "The rejected master plan changed the revision or added a move.");
        Console.WriteLine("PASS: MASTER PLAN REQUIRED FAULT LEAVES HISTORY UNCHANGED");

        (int CardNumber, int PilePosition) move = FindPlacement(first, opening.Hand);
        if (move.PilePosition == 0)
        {
            return false;
        }

        await VerifyPlacementAsync(
            (opening.Lobby, opening.Session), (waiting.Lobby, waiting.Session), (lobbyCode, first, opening.Hand, move));

        IEnumerable<int> expectedMemberCards = isHostOpening
            ? memberHand.Cards
            : memberHand.Cards.Where(card => card != move.CardNumber);

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
        bool outsiderDismantleDenied = false;
        try
        {
            outsiderLobby.DismantleMatchTower(new MatchDismantleRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = outsiderToken,
                PilePosition = 1,
                ExpectedRevision = first.Revision + 1
            });
        }
        catch (FaultException<ServiceFault> exception)
        {
            outsiderDismantleDenied = exception.Detail.ErrorCode == ServiceErrorCodes.LobbyNotMember;
        }

        Require(outsiderDismantleDenied, "An outsider was able to request a dismantle.");
        memberLobby.Disconnect();
        memberLobby.JoinLobby(lobbyCode, memberToken);
        MatchPrivateHand resumedHand = memberLobby.GetMatchPrivateHand(lobbyCode, memberToken);
        Require(resumedHand.MatchId == memberHand.MatchId && resumedHand.PlayerId == memberHand.PlayerId
            && resumedHand.Cards.SequenceEqual(expectedMemberCards), "Private hand changed after reconnecting.");
        MatchBoardMove latest = memberLobby.GetMatchBoard(lobbyCode, memberToken).RecentMoves.Last();
        Require(latest.ActionKind == MatchBoardActionKind.Placement && latest.PlayerId == opening.Session.PlayerId
            && latest.PlayerUsername == opening.Session.Username && latest.PilePosition == move.PilePosition
            && latest.CardNumbers.SequenceEqual(new[] { move.CardNumber }),
            "The reconnected client did not recover the last public move.");
        Console.WriteLine("PASS: PRIVATE HANDS, OUTSIDER DENIAL, RECONNECTION AND RECENT MOVES");
        bool dismantled = await RoundClosureSmoke.VerifyAsync((hostLobby, hostSession), (memberLobby, memberSession),
            lobbyCode);
        if (dismantled)
        {
            Console.WriteLine("PASS: ROUND SUMMARY AFTER RECONNECTION, TURN EXPIRY AND ABANDONMENT FOR BOTH CLIENTS");
        }

        return dismantled;
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
    (ILobbyClient Lobby, UserSession Session) active,
    (ILobbyClient Lobby, UserSession Session) waiting,
    (string LobbyCode, MatchBoardSnapshot Board, MatchPrivateHand Hand,
        (int CardNumber, int PilePosition) Move) opening)
{
    MatchBoardSnapshot initial = opening.Board;
    TaskCompletionSource<MatchBoardChanged> activeUpdate = new(TaskCreationOptions.RunContinuationsAsynchronously);
    TaskCompletionSource<MatchBoardChanged> waitingUpdate = new(TaskCreationOptions.RunContinuationsAsynchronously);
    active.Lobby.MatchBoardUpdated += RecordActiveUpdate;
    waiting.Lobby.MatchBoardUpdated += RecordWaitingUpdate;

    try
    {
        MatchPlaceRequest request = new()
        {
            LobbyCode = opening.LobbyCode,
            SessionToken = active.Session.Token,
            CardNumber = opening.Move.CardNumber,
            PilePosition = opening.Move.PilePosition,
            ExpectedRevision = initial.Revision
        };
        MatchBoardSnapshot placed = active.Lobby.PlaceMatchCard(request);
        MatchBoardChanged[] changes = await Task.WhenAll(activeUpdate.Task, waitingUpdate.Task)
            .WaitAsync(TimeSpan.FromSeconds(15));

        Require(placed.Revision == initial.Revision + 1 && changes.All(change =>
            change.LobbyCode == opening.LobbyCode && change.Revision == placed.Revision
            && change.PlayerUsername == active.Session.Username
            && change.CardNumber == request.CardNumber && change.PilePosition == request.PilePosition),
            "Placement did not notify both clients of the expected move.");

        MatchBoardSnapshot activeBoard = active.Lobby.GetMatchBoard(request.LobbyCode, active.Session.Token);
        MatchBoardSnapshot waitingBoard = waiting.Lobby.GetMatchBoard(request.LobbyCode, waiting.Session.Token);
        foreach (MatchBoardSnapshot board in new[] { placed, activeBoard, waitingBoard })
        {
            Require(board.MatchId == initial.MatchId && board.Revision == placed.Revision
                && board.CurrentTurnPlayerId == waiting.Session.PlayerId
                && board.Piles.Single(pile => pile.Position == request.PilePosition).TopCardNumber
                    == request.CardNumber,
                "A client received an incorrect board after the move.");
            Require(board.Players.Count == 2
                && board.Players.Any(player => player.PlayerId == active.Session.PlayerId
                    && player.Username == active.Session.Username)
                && board.Players.Any(player => player.PlayerId == waiting.Session.PlayerId
                    && player.Username == waiting.Session.Username),
                "A board snapshot did not preserve both player names.");
        }

        MatchPrivateHand hand = active.Lobby.GetMatchPrivateHand(request.LobbyCode, active.Session.Token);
        Require(hand.MatchId == initial.MatchId && hand.PlayerId == active.Session.PlayerId
            && hand.Cards.SequenceEqual(opening.Hand.Cards.Where(card => card != request.CardNumber)),
            "The opening player's hand did not lose exactly the played card.");
        Console.WriteLine("PASS: VALID PLACEMENT, BOARD NOTIFICATIONS, PLAYER NAMES AND PRIVATE HAND");
    }
    finally
    {
        active.Lobby.MatchBoardUpdated -= RecordActiveUpdate;
        waiting.Lobby.MatchBoardUpdated -= RecordWaitingUpdate;
    }

    void RecordActiveUpdate(object? sender, MatchBoardChanged change)
    {
        if (change.MatchId == initial.MatchId)
        {
            activeUpdate.TrySetResult(change);
        }
    }

    void RecordWaitingUpdate(object? sender, MatchBoardChanged change)
    {
        if (change.MatchId == initial.MatchId)
        {
            waitingUpdate.TrySetResult(change);
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
