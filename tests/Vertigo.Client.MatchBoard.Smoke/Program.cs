using System.ServiceModel;
using System.Threading.Channels;
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
    await RunExpulsionVoteAsync();
    await RunMatchExitAsync();
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
        Require(first.ServerTimeUtc.HasValue && first.ServerTimeUtc < first.TurnDeadlineUtc,
            "The board did not provide a server-time sample for the opening turn.");
        Require(hostHand.Revision == first.Revision && memberHand.Revision == second.Revision,
            "Private hand responses did not carry the expected board revisions.");
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

static async Task RunExpulsionVoteAsync()
{
    using ServiceProvider first = CreateClient();
    using ServiceProvider second = CreateClient();
    using ServiceProvider third = CreateClient();
    using ServiceProvider outsider = CreateClient();
    using ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();
    string outsiderToken = CreateGuest(outsider).Token;
    (ILobbyClient Lobby, UserSession Session)[] clients = new[] { first, second, third }.Select(provider =>
        (provider.GetRequiredService<ILobbyClient>(), CreateGuest(provider))).ToArray();

    ILobbyClient host = clients[0].Lobby;
    string hostToken = clients[0].Session.Token;
    string lobbyCode = host.CreateLobby(true, 3, hostToken);
    foreach ((ILobbyClient Lobby, UserSession Session) client in clients.Skip(1))
    {
        client.Lobby.JoinLobby(lobbyCode, client.Session.Token);
    }

    foreach ((ILobbyClient Lobby, UserSession Session) client in clients)
    {
        client.Lobby.SetReady(lobbyCode, true, client.Session.Token);
    }

    host.StartMatch(lobbyCode, hostToken);
    MatchBoardSnapshot initial = host.GetMatchBoard(lobbyCode, hostToken);
    (ILobbyClient Lobby, UserSession Session) target = clients.First(client =>
        client.Session.PlayerId != initial.CurrentTurnPlayerId);
    (ILobbyClient Lobby, UserSession Session)[] voters = clients.Where(client =>
        client.Session.PlayerId != target.Session.PlayerId).ToArray();
    Channel<MatchBoardChanged> notices = Channel.CreateUnbounded<MatchBoardChanged>();
    target.Lobby.MatchBoardUpdated += ObserveVote;
    try
    {
        MatchExpulsionVoteRequest request = new()
        {
            LobbyCode = lobbyCode,
            MatchId = initial.MatchId,
            SessionToken = voters[0].Session.Token,
            TargetPlayerId = target.Session.PlayerId,
            ExpectedVotingRevision = initial.VotingRevision
        };
        request.MatchId = Guid.NewGuid();
        VerifyRejectedVote(voters[0].Lobby, request, "MATCH_UNAVAILABLE");
        request.MatchId = initial.MatchId;
        request.SessionToken = outsiderToken;
        VerifyRejectedVote(outsiderLobby, request, ServiceErrorCodes.LobbyNotMember);
        request.SessionToken = voters[0].Session.Token;
        MatchBoardSnapshot unchanged = host.GetMatchBoard(lobbyCode, hostToken);
        Require(unchanged.Revision == initial.Revision && unchanged.ExpulsionVotes.Count == 0,
            "A rejected vote changed the board.");
        MatchBoardSnapshot recorded = voters[0].Lobby.VoteToExpelMatchPlayer(request);
        Require(recorded.ExpulsionVotesRequired == 2 && recorded.ExpulsionVotes.Single().VoteCount == 1,
            "The first vote did not produce a pending tally.");
        VerifyRejectedVote(voters[0].Lobby, request, "MATCH_ALREADY_VOTED");
        voters[0].Lobby.Disconnect();
        voters[0].Lobby.JoinLobby(lobbyCode, voters[0].Session.Token);
        MatchPrivateHand resumedVote = voters[0].Lobby.GetMatchPrivateHand(lobbyCode, voters[0].Session.Token);
        MatchPrivateHand otherHand = voters[1].Lobby.GetMatchPrivateHand(lobbyCode, voters[1].Session.Token);
        Require(resumedVote.VotingRevision == recorded.VotingRevision
            && resumedVote.VotedExpulsionTargetIds.SequenceEqual(new[] { target.Session.PlayerId })
            && otherHand.VotedExpulsionTargetIds.Count == 0,
            "A reconnected client did not receive its own vote, or another client received it.");

        request.SessionToken = voters[1].Session.Token;
        MatchBoardSnapshot expelled = voters[1].Lobby.VoteToExpelMatchPlayer(request);
        Require(expelled.ExpelledPlayerIds.SequenceEqual(new[] { target.Session.PlayerId })
            && expelled.ExpulsionVotes.Count == 0 && initial.CurrentTurnPlayerId == expelled.CurrentTurnPlayerId
            && initial.TurnDeadlineUtc == expelled.TurnDeadlineUtc,
            "A vote against a waiting player did not preserve the active turn.");
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));
        MatchBoardChanged firstNotice = await notices.Reader.ReadAsync(timeout.Token);
        MatchBoardChanged secondNotice = await notices.Reader.ReadAsync(timeout.Token);
        Require(firstNotice.ActionKind == MatchBoardActionKind.ExpulsionVoteRecorded
            && firstNotice.Revision == recorded.Revision && firstNotice.PlayerUsername == string.Empty
            && secondNotice.ActionKind == MatchBoardActionKind.ExpelledByVote
            && secondNotice.Revision == expelled.Revision && secondNotice.PlayerUsername == target.Session.Username,
            "The expelled client did not receive the expected vote notifications.");
        MatchLeaveRequest leave = new()
        {
            LobbyCode = lobbyCode,
            MatchId = expelled.MatchId,
            SessionToken = target.Session.Token
        };
        Require(target.Lobby.LeaveMatch(leave) && target.Lobby.GetCurrentLobby(target.Session.Token) is null,
            "The expelled player could not leave the match.");
        MatchBoardSnapshot afterExit = voters[0].Lobby.GetMatchBoard(lobbyCode, voters[0].Session.Token);
        Require(afterExit.Revision == expelled.Revision && afterExit.EndReason == MatchEndReason.None
            && afterExit.CurrentTurnPlayerId == expelled.CurrentTurnPlayerId
            && afterExit.TurnDeadlineUtc == expelled.TurnDeadlineUtc && afterExit.Players.Count == 2,
            "Leaving after expulsion changed the match for the remaining players.");
        Console.WriteLine("PASS: EXPELLED PLAYER CAN LEAVE WITHOUT CHANGING THE ONGOING MATCH");
        Console.WriteLine("PASS: EXPULSION VOTE OVER NETTCP PRESERVES THE ACTIVE TURN");
    }
    finally
    {
        target.Lobby.MatchBoardUpdated -= ObserveVote;
        foreach ((ILobbyClient Lobby, UserSession Session) client in clients)
        {
            client.Lobby.Disconnect();
        }

        foreach (ServiceProvider provider in new[] { first, second, third, outsider })
        {
            provider.GetRequiredService<ISessionCoordinator>().Logout();
        }
    }

    void ObserveVote(object? sender, MatchBoardChanged notice)
    {
        notices.Writer.TryWrite(notice);
    }
}

static void VerifyRejectedVote(ILobbyClient client, MatchExpulsionVoteRequest request, string expectedError)
{
    bool rejected = false;
    try
    {
        client.VoteToExpelMatchPlayer(request);
    }
    catch (FaultException<ServiceFault> exception)
    {
        rejected = exception.Detail.ErrorCode == expectedError;
    }

    Require(rejected, $"The server did not reject the vote with {expectedError}.");
}

static async Task RunMatchExitAsync()
{
    using ServiceProvider first = CreateClient();
    using ServiceProvider second = CreateClient();
    ILobbyClient leaving = first.GetRequiredService<ILobbyClient>();
    ILobbyClient staying = second.GetRequiredService<ILobbyClient>();
    UserSession firstSession = CreateGuest(first);
    UserSession secondSession = CreateGuest(second);

    try
    {
        string code = leaving.CreateLobby(true, 2, firstSession.Token);
        staying.JoinLobby(code, secondSession.Token);
        leaving.SetReady(code, true, firstSession.Token);
        staying.SetReady(code, true, secondSession.Token);
        Guid matchId = leaving.StartMatch(code, firstSession.Token).MatchId;
        MatchLeaveRequest request = new() { MatchId = matchId, LobbyCode = code, SessionToken = firstSession.Token };
        TaskCompletionSource<MatchBoardChanged> leftNotice = new(TaskCreationOptions.RunContinuationsAsynchronously);
        staying.MatchBoardUpdated += ObserveDeparture;
        try
        {
            request.MatchId = Guid.NewGuid();
            Require(!leaving.LeaveMatch(request), "An exit request was accepted for the wrong match.");
            request.MatchId = matchId;
            Require(leaving.LeaveMatch(request), "The active player could not leave.");
            MatchBoardChanged notice = await leftNotice.Task.WaitAsync(TimeSpan.FromSeconds(15));
            Require(notice.MatchId == matchId && notice.PlayerUsername == firstSession.Username,
                "The remaining client did not receive the departure notification.");
            Require(leaving.GetCurrentLobby(firstSession.Token) is null, "The departure did not clear membership.");
            Require(leaving.LeaveMatch(request), "Retrying the completed departure failed.");
            Require(staying.GetMatchBoard(code, secondSession.Token).Revision == notice.Revision,
                "Retrying the departure changed the board.");

            bool rejoinDenied = false;
            try
            {
                leaving.JoinLobby(code, firstSession.Token);
            }
            catch (FaultException<ServiceFault> exception)
            {
                rejoinDenied = exception.Detail.ErrorCode == ServiceErrorCodes.LobbyAlreadyStarted;
            }

            Require(rejoinDenied, "The departed player was able to rejoin the old match.");
            staying.Disconnect();
            staying.JoinLobby(code, secondSession.Token);
            MatchBoardSnapshot result = staying.GetMatchBoard(code, secondSession.Token);
            Require(result.EndReason == MatchEndReason.Abandonment
                && result.WinnerIds.SequenceEqual(new[] { secondSession.PlayerId })
                && result.RecentMoves.Single().PlayerUsername == firstSession.Username,
                "After reconnecting, the remaining client did not recover the winner and departure.");

            request.SessionToken = secondSession.Token;
            Require(staying.LeaveMatch(request) && staying.LeaveMatch(request),
                "Leaving a finished match or retrying the request failed.");
            Require(staying.GetCurrentLobby(secondSession.Token) is null, "Finished match membership was retained.");
            string next = staying.CreateLobby(true, 2, secondSession.Token);
            leaving.JoinLobby(next, firstSession.Token);
            Console.WriteLine("PASS: VOLUNTARY DEPARTURE, FINAL RESULT AFTER RECONNECT AND NEW LOBBY");
        }
        finally
        {
            staying.MatchBoardUpdated -= ObserveDeparture;
        }

        void ObserveDeparture(object? sender, MatchBoardChanged change)
        {
            if (change.ActionKind == MatchBoardActionKind.LeftMatch)
            {
                leftNotice.TrySetResult(change);
            }
        }
    }
    finally
    {
        leaving.Disconnect();
        staying.Disconnect();
        first.GetRequiredService<ISessionCoordinator>().Logout();
        second.GetRequiredService<ISessionCoordinator>().Logout();
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
