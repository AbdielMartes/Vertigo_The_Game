using System.ServiceModel;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;

const int MaximumMatchAttempts = 12;

int exitCode = 0;
try
{
    bool played = false;
    for (int attempt = 0; attempt < MaximumMatchAttempts && !played; attempt++)
    {
        played = await RunMatchAsync();
    }

    Require(played, "No attempt exercised both an opening placement and a dismantle before round two.");
    await RunMatchChatAsync();
    await RunExpulsionVoteAsync();
    await RunMatchExitAsync();
}
catch (FaultException<ServiceFault> exception)
{
    Console.Error.WriteLine($"FAIL BUSINESS: {exception.Detail.ErrorCode}; Key={exception.Detail.MessageKey}");
    exitCode = 1;
}
catch (TimeoutException)
{
    Console.Error.WriteLine("FAIL TIMEOUT: GetMatchBoard smoke timed out.");
    exitCode = 1;
}
catch (CommunicationException exception)
{
    Console.Error.WriteLine($"FAIL TRANSPORT: {exception.GetType().Name}");
    exitCode = 1;
}
catch (Exception exception)
{
    Console.Error.WriteLine($"FAIL UNEXPECTED: {exception.GetType().Name} (0x{exception.HResult:X8})");
    exitCode = 1;
}

return exitCode;

static async Task<bool> RunMatchAsync()
{
    using ServiceProvider host = CreateClient();
    using ServiceProvider member = CreateClient();
    using ServiceProvider outsider = CreateClient();
    using ILobbyClient hostLobby = host.GetRequiredService<ILobbyClient>();
    using ILobbyClient memberLobby = member.GetRequiredService<ILobbyClient>();
    using ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();

    bool dismantled = false;
    try
    {
        UserSession hostSession = CreateGuest(host);
        UserSession memberSession = CreateGuest(member);
        SmokeParticipant hostPlayer = new(hostLobby, hostSession);
        SmokeParticipant memberPlayer = new(memberLobby, memberSession);
        (string lobbyCode, Guid matchId) = StartSmokeMatch(hostPlayer, new[] { memberPlayer });
        (MatchBoardSnapshot first, MatchBoardSnapshot second) =
            VerifyOpeningBoards(hostPlayer, memberPlayer, (lobbyCode, matchId));
        (MatchPrivateHand hostHand, MatchPrivateHand memberHand) =
            VerifyOpeningHands(hostPlayer, memberPlayer, (lobbyCode, first, second));

        bool isHostOpening = first.CurrentTurnPlayerId == hostSession.PlayerId;
        (SmokeParticipant Player, MatchPrivateHand Hand) opening = isHostOpening
            ? (hostPlayer, hostHand)
            : (memberPlayer, memberHand);
        (SmokeParticipant Player, MatchPrivateHand Hand) waiting = isHostOpening
            ? (memberPlayer, memberHand)
            : (hostPlayer, hostHand);

        VerifyWrongTurn(waiting, hostPlayer, (lobbyCode, second.Revision, first.Revision));
        VerifyUnearnedMasterPlan(opening, first, lobbyCode);

        (int CardNumber, int PilePosition) move = FindPlacement(first, opening.Hand);
        if (move.PilePosition != 0)
        {
            await VerifyPlacementAsync(
                opening.Player, waiting.Player, (lobbyCode, first, opening.Hand, move));

            IEnumerable<int> expectedMemberCards = isHostOpening
                ? memberHand.Cards
                : memberHand.Cards.Where(card => card != move.CardNumber);

            UserSession outsiderSession = CreateGuest(outsider);
            VerifyOutsiderDenial(new SmokeParticipant(outsiderLobby, outsiderSession),
                lobbyCode, first.Revision + 1);
            VerifyReconnectedMember(memberPlayer, (lobbyCode, memberHand, expectedMemberCards),
                (opening.Player.Session, move));

            dismantled = await RoundClosureSmoke.VerifyAsync((hostLobby, hostSession), (memberLobby, memberSession),
                lobbyCode);
            if (dismantled)
            {
                Console.WriteLine("PASS: ROUND SUMMARY AFTER RECONNECTION, TURN EXPIRY AND ABANDONMENT FOR BOTH CLIENTS");
            }
        }
    }
    finally
    {
        hostLobby.Disconnect();
        memberLobby.Disconnect();
        host.GetRequiredService<ISessionCoordinator>().Logout();
        member.GetRequiredService<ISessionCoordinator>().Logout();
        outsider.GetRequiredService<ISessionCoordinator>().Logout();
    }

    return dismantled;
}

static (string LobbyCode, Guid MatchId) StartSmokeMatch(
    SmokeParticipant host, IReadOnlyList<SmokeParticipant> members)
{
    string hostToken = host.Session.Token;
    string lobbyCode = host.Lobby.CreateLobby(true, members.Count + 1, hostToken);
    foreach (SmokeParticipant member in members)
    {
        member.Lobby.JoinLobby(lobbyCode, member.Session.Token);
    }

    host.Lobby.SetReady(lobbyCode, true, hostToken);
    foreach (SmokeParticipant member in members)
    {
        member.Lobby.SetReady(lobbyCode, true, member.Session.Token);
    }

    LobbyPreparation started = host.Lobby.StartMatch(lobbyCode, hostToken);
    return (lobbyCode, started.MatchId);
}

static (MatchBoardSnapshot Host, MatchBoardSnapshot Member) VerifyOpeningBoards(
    SmokeParticipant host, SmokeParticipant member, (string LobbyCode, Guid MatchId) match)
{
    string lobbyCode = match.LobbyCode;
    Guid matchId = match.MatchId;
    Require(matchId != Guid.Empty, "StartMatch returned an empty MatchId.");

    LobbyPreparation? membership = member.Lobby.GetCurrentLobby(member.Session.Token);
    Require(membership?.LobbyCode == lobbyCode && membership.MatchId == matchId,
        "Current lobby does not match the running match.");

    MatchBoardSnapshot first = host.Lobby.GetMatchBoard(lobbyCode, host.Session.Token);
    MatchBoardSnapshot second = member.Lobby.GetMatchBoard(lobbyCode, member.Session.Token);
    Require(first.MatchId == matchId, "Host board has wrong MatchId.");
    Require(second.MatchId == matchId, "Member board has wrong MatchId.");
    Require(first.Players.Count == 2 && second.Players.Count == 2,
        "Match board did not return two participants.");
    Require(first.Piles.Count == 1 && second.Piles.Count == 1
        && first.Piles[0].TopCardNumber == second.Piles[0].TopCardNumber,
        "Both clients must start with the same single tower.");
    Console.WriteLine("PASS: GET MATCH BOARD OVER NETTCP FOR BOTH PARTICIPANTS");
    return (first, second);
}

static (MatchPrivateHand Host, MatchPrivateHand Member) VerifyOpeningHands(
    SmokeParticipant host, SmokeParticipant member,
    (string LobbyCode, MatchBoardSnapshot Host, MatchBoardSnapshot Member) match)
{
    string lobbyCode = match.LobbyCode;
    MatchBoardSnapshot first = match.Host;
    MatchBoardSnapshot second = match.Member;
    MatchPrivateHand hostHand = host.Lobby.GetMatchPrivateHand(lobbyCode, host.Session.Token);
    MatchPrivateHand memberHand = member.Lobby.GetMatchPrivateHand(lobbyCode, member.Session.Token);
    Require(first.ServerTimeUtc.HasValue && first.ServerTimeUtc < first.TurnDeadlineUtc,
        "The board did not provide a server-time sample for the opening turn.");
    Require(hostHand.Revision == first.Revision && memberHand.Revision == second.Revision,
        "Private hand responses did not carry the expected board revisions.");
    Require(hostHand.MatchId == first.MatchId && memberHand.MatchId == first.MatchId,
        "Private hands have the wrong match identifier.");
    Require(hostHand.PlayerId == host.Session.PlayerId && memberHand.PlayerId == member.Session.PlayerId
        && hostHand.Cards.Count == 10 && memberHand.Cards.Count == 10
        && hostHand.Cards.Concat(memberHand.Cards).Distinct().Count() == 20,
        "Private hands overlap or are invalid.");
    Require(first.CurrentTurnPlayerId == second.CurrentTurnPlayerId
        && first.Players.Any(player => player.PlayerId == first.CurrentTurnPlayerId),
        "Clients disagree on the opening turn or identify a non-participant.");
    return (hostHand, memberHand);
}

static void VerifyWrongTurn(
    (SmokeParticipant Player, MatchPrivateHand Hand) waiting,
    SmokeParticipant host, (string LobbyCode, long AttemptRevision, long HostRevision) match)
{
    bool wrongTurnDenied = false;
    try
    {
        waiting.Player.Lobby.PlaceMatchCard(new MatchPlaceRequest
        {
            LobbyCode = match.LobbyCode,
            SessionToken = waiting.Player.Session.Token,
            CardNumber = waiting.Hand.Cards[0],
            PilePosition = 1,
            ExpectedRevision = match.AttemptRevision
        });
    }
    catch (FaultException<ServiceFault> exception)
    {
        wrongTurnDenied = exception.Detail.MessageKey == "Match.Error.NotYourTurn";
    }

    Require(wrongTurnDenied, "Placement outside the player's turn was not rejected.");
    Require(host.Lobby.GetMatchBoard(match.LobbyCode, host.Session.Token).Revision == match.HostRevision,
        "Rejected placement changed the board revision.");
    Console.WriteLine("PASS: WRONG TURN REJECTED WITHOUT CHANGING BOARD");
}

static void VerifyUnearnedMasterPlan(
    (SmokeParticipant Player, MatchPrivateHand Hand) opening,
    MatchBoardSnapshot first, string lobbyCode)
{
    bool planRequired = false;
    try
    {
        opening.Player.Lobby.UseMatchMasterPlan(new MatchMasterPlanRequest
        {
            LobbyCode = lobbyCode,
            SessionToken = opening.Player.Session.Token,
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
    MatchBoardSnapshot afterRejectedPlan = opening.Player.Lobby.GetMatchBoard(lobbyCode, opening.Player.Session.Token);
    Require(afterRejectedPlan.Revision == first.Revision && afterRejectedPlan.RecentMoves.Count == 0,
        "The rejected master plan changed the revision or added a move.");
    Console.WriteLine("PASS: MASTER PLAN REQUIRED FAULT LEAVES HISTORY UNCHANGED");
}

static void VerifyOutsiderDenial(SmokeParticipant outsider, string lobbyCode, long revision)
{
    bool outsiderDenied = false;
    try
    {
        outsider.Lobby.GetMatchPrivateHand(lobbyCode, outsider.Session.Token);
    }
    catch (FaultException<ServiceFault> exception)
    {
        outsiderDenied = exception.Detail.ErrorCode == ServiceErrorCodes.LobbyNotMember;
    }

    Require(outsiderDenied, "An outsider was able to read the private hand.");
    bool outsiderDismantleDenied = false;
    try
    {
        outsider.Lobby.DismantleMatchTower(new MatchDismantleRequest
        {
            LobbyCode = lobbyCode,
            SessionToken = outsider.Session.Token,
            PilePosition = 1,
            ExpectedRevision = revision
        });
    }
    catch (FaultException<ServiceFault> exception)
    {
        outsiderDismantleDenied = exception.Detail.ErrorCode == ServiceErrorCodes.LobbyNotMember;
    }

    Require(outsiderDismantleDenied, "An outsider was able to request a dismantle.");
}

static void VerifyReconnectedMember(
    SmokeParticipant member,
    (string LobbyCode, MatchPrivateHand OriginalHand, IEnumerable<int> ExpectedCards) state,
    (UserSession Session, (int CardNumber, int PilePosition) Move) opening)
{
    member.Lobby.Disconnect();
    member.Lobby.JoinLobby(state.LobbyCode, member.Session.Token);
    MatchPrivateHand resumedHand = member.Lobby.GetMatchPrivateHand(state.LobbyCode, member.Session.Token);
    Require(resumedHand.MatchId == state.OriginalHand.MatchId && resumedHand.PlayerId == state.OriginalHand.PlayerId
        && resumedHand.Cards.SequenceEqual(state.ExpectedCards), "Private hand changed after reconnecting.");
    MatchBoardMove latest = member.Lobby.GetMatchBoard(state.LobbyCode, member.Session.Token).RecentMoves.Last();
    Require(latest.ActionKind == MatchBoardActionKind.Placement && latest.PlayerId == opening.Session.PlayerId
        && latest.PlayerUsername == opening.Session.Username && latest.PilePosition == opening.Move.PilePosition
        && latest.CardNumbers.SequenceEqual(new[] { opening.Move.CardNumber }),
        "The reconnected client did not recover the last public move.");
    Console.WriteLine("PASS: PRIVATE HANDS, OUTSIDER DENIAL, RECONNECTION AND RECENT MOVES");
}

static async Task RunMatchChatAsync()
{
    using ServiceProvider host = CreateClient();
    using ServiceProvider member = CreateClient();
    using ServiceProvider outsider = CreateClient();
    SmokeParticipant hostPlayer = new(host.GetRequiredService<ILobbyClient>(), CreateGuest(host));
    SmokeParticipant memberPlayer = new(member.GetRequiredService<ILobbyClient>(), CreateGuest(member));
    UserSession outsiderSession = CreateGuest(outsider);
    (string lobbyCode, Guid matchId) = StartSmokeMatch(hostPlayer, new[] { memberPlayer });
    MatchChatRequest request = new()
    {
        LobbyCode = lobbyCode, MatchId = matchId, Text = new string('a', 30),
        SessionToken = hostPlayer.Session.Token
    };
    IChatClient chat = host.GetRequiredService<IChatClient>();
    VerifyMatchChatRejections(chat, request, outsiderSession.Token);
    await VerifyMatchChatDeliveryAsync(chat, request, (hostPlayer, memberPlayer));
    VerifyFinishedMatchChat(chat, request, memberPlayer);
    Console.WriteLine("PASS: MATCH CHAT SCOPE, VALIDATION AND CALLBACKS");
}

static void VerifyFinishedMatchChat(IChatClient chat, MatchChatRequest request, SmokeParticipant member)
{
    Require(member.Lobby.LeaveMatch(new MatchLeaveRequest
    {
        LobbyCode = request.LobbyCode,
        MatchId = request.MatchId,
        SessionToken = member.Session.Token
    }), "The member could not leave the chat test match.");
    Require(chat.SendMatchMessage(request).ErrorCode == ServiceErrorCodes.LobbyChatNotAllowed,
        "The remaining player could still chat after the match ended.");
}

static void VerifyMatchChatRejections(IChatClient chat, MatchChatRequest request, string outsiderToken)
{
    string token = request.SessionToken;
    request.SessionToken = outsiderToken;
    Require(chat.SendMatchMessage(request).ErrorCode == ServiceErrorCodes.LobbyChatNotAllowed,
        "The server accepted match chat from an outsider.");
    request.SessionToken = token;

    Guid matchId = request.MatchId;
    request.MatchId = Guid.NewGuid();
    Require(chat.SendMatchMessage(request).ErrorCode == ServiceErrorCodes.LobbyChatNotAllowed,
        "The server accepted match chat for an unrelated match.");
    request.MatchId = matchId;

    string text = request.Text;
    request.Text += "b";
    Require(chat.SendMatchMessage(request).ErrorCode == ServiceErrorCodes.InvalidData,
        "The server accepted a match message exceeding 30 characters.");
    request.Text = text;
    Require(chat.SendMessage(request.LobbyCode, request.Text, token).ErrorCode == ServiceErrorCodes.LobbyChatNotAllowed,
        "The waiting-room endpoint bypassed match chat scoping.");
}

static async Task VerifyMatchChatDeliveryAsync(IChatClient chat, MatchChatRequest request,
    (SmokeParticipant Sender, SmokeParticipant Receiver) players)
{
    Channel<LobbyChatMessage> senderMessages = Channel.CreateUnbounded<LobbyChatMessage>();
    Channel<LobbyChatMessage> receiverMessages = Channel.CreateUnbounded<LobbyChatMessage>();
    players.Sender.Lobby.MessageReceived += OnSenderMessage;
    players.Receiver.Lobby.MessageReceived += OnReceiverMessage;
    try
    {
        Require(chat.SendMatchMessage(request).Value, "The server rejected a 30-character match message.");
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));
        LobbyChatMessage senderMessage = await senderMessages.Reader.ReadAsync(timeout.Token);
        LobbyChatMessage receiverMessage = await receiverMessages.Reader.ReadAsync(timeout.Token);
        foreach (LobbyChatMessage message in new[] { senderMessage, receiverMessage })
        {
            Require(message.MatchId == request.MatchId && message.LobbyCode == request.LobbyCode
                && message.Text == request.Text && message.SenderId == players.Sender.Session.PlayerId,
                "The callback delivered a different match message.");
        }
    }
    finally
    {
        players.Sender.Lobby.MessageReceived -= OnSenderMessage;
        players.Receiver.Lobby.MessageReceived -= OnReceiverMessage;
    }

    void OnSenderMessage(object? sender, LobbyChatMessage message) => senderMessages.Writer.TryWrite(message);

    void OnReceiverMessage(object? sender, LobbyChatMessage message) => receiverMessages.Writer.TryWrite(message);
}

static async Task RunExpulsionVoteAsync()
{
    using ServiceProvider first = CreateClient();
    using ServiceProvider second = CreateClient();
    using ServiceProvider third = CreateClient();
    using ServiceProvider outsider = CreateClient();
    using ILobbyClient outsiderLobby = outsider.GetRequiredService<ILobbyClient>();
    string outsiderToken = CreateGuest(outsider).Token;
    SmokeParticipant[] clients = new[] { first, second, third }.Select(provider =>
        new SmokeParticipant(provider.GetRequiredService<ILobbyClient>(), CreateGuest(provider))).ToArray();

    SmokeParticipant host = clients[0];
    string lobbyCode = StartSmokeMatch(host, clients[1..]).LobbyCode;
    MatchBoardSnapshot initial = host.Lobby.GetMatchBoard(lobbyCode, host.Session.Token);
    SmokeParticipant target = clients.First(client =>
        client.Session.PlayerId != initial.CurrentTurnPlayerId);
    SmokeParticipant[] voters = clients.Where(client =>
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
        VerifyRejectedVotes(request, voters[0], (outsiderLobby, outsiderToken));
        MatchBoardSnapshot unchanged = host.Lobby.GetMatchBoard(lobbyCode, host.Session.Token);
        Require(unchanged.Revision == initial.Revision && unchanged.ExpulsionVotes.Count == 0,
            "A rejected vote changed the board.");
        MatchBoardSnapshot recorded = RecordFirstVote(request, voters, target.Session.PlayerId);

        request.SessionToken = voters[1].Session.Token;
        MatchBoardSnapshot expelled = voters[1].Lobby.VoteToExpelMatchPlayer(request);
        Require(expelled.ExpelledPlayerIds.SequenceEqual(new[] { target.Session.PlayerId })
            && expelled.ExpulsionVotes.Count == 0 && initial.CurrentTurnPlayerId == expelled.CurrentTurnPlayerId
            && initial.TurnDeadlineUtc == expelled.TurnDeadlineUtc,
            "A vote against a waiting player did not preserve the active turn.");
        await VerifyVoteNoticesAsync(notices.Reader, (recorded, expelled), target.Session.Username);
        IChatClient chat = first.GetRequiredService<IChatClient>();
        Require(chat.SendMatchMessage(new MatchChatRequest
        {
            LobbyCode = lobbyCode,
            MatchId = expelled.MatchId,
            SessionToken = target.Session.Token,
            Text = "Test message"
        }).ErrorCode == ServiceErrorCodes.LobbyChatNotAllowed, "An expelled member could still use match chat.");
        VerifyExpelledPlayerExit(target, voters[0], (lobbyCode, expelled));
        Console.WriteLine("PASS: EXPULSION VOTE OVER NETTCP PRESERVES THE ACTIVE TURN");
    }
    finally
    {
        target.Lobby.MatchBoardUpdated -= ObserveVote;
        foreach (SmokeParticipant client in clients)
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

static void VerifyRejectedVotes(
    MatchExpulsionVoteRequest request, SmokeParticipant voter,
    (ILobbyClient Lobby, string Token) outsider)
{
    Guid matchId = request.MatchId;
    request.MatchId = Guid.NewGuid();
    VerifyRejectedVote(voter.Lobby, request, "MATCH_UNAVAILABLE");
    request.MatchId = matchId;
    request.SessionToken = outsider.Token;
    VerifyRejectedVote(outsider.Lobby, request, ServiceErrorCodes.LobbyNotMember);
    request.SessionToken = voter.Session.Token;
}

static MatchBoardSnapshot RecordFirstVote(
    MatchExpulsionVoteRequest request, SmokeParticipant[] voters, Guid targetId)
{
    string lobbyCode = request.LobbyCode;
    MatchBoardSnapshot recorded = voters[0].Lobby.VoteToExpelMatchPlayer(request);
    Require(recorded.ExpulsionVotesRequired == 2 && recorded.ExpulsionVotes.Single().VoteCount == 1,
        "The first vote did not produce a pending tally.");
    VerifyRejectedVote(voters[0].Lobby, request, "MATCH_ALREADY_VOTED");
    voters[0].Lobby.Disconnect();
    voters[0].Lobby.JoinLobby(lobbyCode, voters[0].Session.Token);
    MatchPrivateHand resumedVote = voters[0].Lobby.GetMatchPrivateHand(lobbyCode, voters[0].Session.Token);
    MatchPrivateHand otherHand = voters[1].Lobby.GetMatchPrivateHand(lobbyCode, voters[1].Session.Token);
    Require(resumedVote.VotingRevision == recorded.VotingRevision
        && resumedVote.VotedExpulsionTargetIds.SequenceEqual(new[] { targetId })
        && otherHand.VotedExpulsionTargetIds.Count == 0,
        "A reconnected client did not receive its own vote, or another client received it.");
    return recorded;
}

static async Task VerifyVoteNoticesAsync(
    ChannelReader<MatchBoardChanged> notices,
    (MatchBoardSnapshot Recorded, MatchBoardSnapshot Expelled) boards, string targetUsername)
{
    using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(15));
    MatchBoardChanged firstNotice = await notices.ReadAsync(timeout.Token);
    MatchBoardChanged secondNotice = await notices.ReadAsync(timeout.Token);
    Require(firstNotice.ActionKind == MatchBoardActionKind.ExpulsionVoteRecorded
        && firstNotice.Revision == boards.Recorded.Revision && firstNotice.PlayerUsername == string.Empty
        && secondNotice.ActionKind == MatchBoardActionKind.ExpelledByVote
        && secondNotice.Revision == boards.Expelled.Revision && secondNotice.PlayerUsername == targetUsername,
        "The expelled client did not receive the expected vote notifications.");
}

static void VerifyExpelledPlayerExit(
    SmokeParticipant target,
    SmokeParticipant remaining,
    (string LobbyCode, MatchBoardSnapshot Board) result)
{
    string lobbyCode = result.LobbyCode;
    MatchBoardSnapshot expelled = result.Board;
    MatchLeaveRequest leave = new()
    {
        LobbyCode = lobbyCode,
        MatchId = expelled.MatchId,
        SessionToken = target.Session.Token
    };
    Require(target.Lobby.LeaveMatch(leave) && target.Lobby.GetCurrentLobby(target.Session.Token) is null,
        "The expelled player could not leave the match.");
    MatchBoardSnapshot afterExit = remaining.Lobby.GetMatchBoard(lobbyCode, remaining.Session.Token);
    Require(afterExit.Revision == expelled.Revision && afterExit.EndReason == MatchEndReason.None
        && afterExit.CurrentTurnPlayerId == expelled.CurrentTurnPlayerId
        && afterExit.TurnDeadlineUtc == expelled.TurnDeadlineUtc && afterExit.Players.Count == 2,
        "Leaving after expulsion changed the match for the remaining players.");
    Console.WriteLine("PASS: EXPELLED PLAYER CAN LEAVE WITHOUT CHANGING THE ONGOING MATCH");
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
    SmokeParticipant active, SmokeParticipant waiting,
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

        VerifyPlacementNotices(changes, (initial.Revision, placed.Revision), (request, active.Session.Username));

        MatchBoardSnapshot activeBoard = active.Lobby.GetMatchBoard(request.LobbyCode, active.Session.Token);
        MatchBoardSnapshot waitingBoard = waiting.Lobby.GetMatchBoard(request.LobbyCode, waiting.Session.Token);
        foreach (MatchBoardSnapshot board in new[] { placed, activeBoard, waitingBoard })
        {
            VerifyPlacedBoard(board, (initial.MatchId, placed.Revision, waiting.Session.PlayerId), request);
            VerifyPlacedBoardPlayers(board, active.Session, waiting.Session);
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

static void VerifyPlacementNotices(
    MatchBoardChanged[] changes, (long Before, long After) revision,
    (MatchPlaceRequest Request, string Username) placement)
{
    MatchPlaceRequest request = placement.Request;
    Require(revision.After == revision.Before + 1 && changes.All(change =>
        change.LobbyCode == request.LobbyCode && change.Revision == revision.After
        && change.PlayerUsername == placement.Username
        && change.CardNumber == request.CardNumber && change.PilePosition == request.PilePosition),
        "Placement did not notify both clients of the expected move.");
}

static void VerifyPlacedBoard(
    MatchBoardSnapshot board, (Guid MatchId, long Revision, Guid NextPlayerId) expected, MatchPlaceRequest request)
{
    Require(board.MatchId == expected.MatchId && board.Revision == expected.Revision
        && board.CurrentTurnPlayerId == expected.NextPlayerId
        && board.Piles.Single(pile => pile.Position == request.PilePosition).TopCardNumber
            == request.CardNumber,
        "A client received an incorrect board after the move.");
}

static void VerifyPlacedBoardPlayers(MatchBoardSnapshot board, UserSession active, UserSession waiting)
{
    Require(board.Players.Count == 2
        && board.Players.Any(player => player.PlayerId == active.PlayerId
            && player.Username == active.Username)
        && board.Players.Any(player => player.PlayerId == waiting.PlayerId
            && player.Username == waiting.Username),
        "A board snapshot did not preserve both player names.");
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

internal sealed record SmokeParticipant(ILobbyClient Lobby, UserSession Session);
