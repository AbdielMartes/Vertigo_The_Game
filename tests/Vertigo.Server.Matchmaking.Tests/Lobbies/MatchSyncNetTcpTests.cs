using System.Runtime.Serialization;
using System.ServiceModel;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Xunit.Abstractions;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class MatchSyncNetTcpTests
{
    private readonly ITestOutputHelper _output;

    public MatchSyncNetTcpTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GetMatchUpdate_AlreadyCurrent_OmitsGroupsAndHand()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        (MatchSyncRequest request, Guid playerId) = OpenMatch(server);
        MatchSyncUpdate full = server.Channel.GetMatchUpdate(request);
        MatchSyncState before = MatchSynchronization.Apply(full, null, playerId);
        long fullBytes = Bytes(request) + Bytes(full);
        request.Known = MatchSynchronization.Describe(before);

        MatchSyncUpdate delta = server.Channel.GetMatchUpdate(request);
        MatchSyncState after = MatchSynchronization.Apply(delta, before, playerId);

        Assert.Equal(full.Revision, delta.BaseRevision);
        Assert.Null(delta.Players);
        Assert.Null(delta.Piles);
        Assert.Null(delta.Rounds);
        Assert.Null(delta.Status);
        Assert.Null(delta.RecentMoves);
        Assert.Null(delta.Hand);
        Assert.False(delta.HandChanged);
        Assert.Equal(before.Hand!.Cards, after.Hand!.Cards);
        Assert.Equal(before.Board.TurnDeadlineUtc, after.Board.TurnDeadlineUtc);
        Assert.True(after.Board.ServerTimeUtc >= before.Board.ServerTimeUtc);
        long deltaBytes = Bytes(request) + Bytes(delta);
        _output.WriteLine($"Same revision, request + response XML data models: {fullBytes} → {deltaBytes} bytes.");
        Assert.True(deltaBytes < fullBytes);
    }

    [Fact]
    public async Task GetMatchUpdate_ClientAhead_SendsFullState()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        (MatchSyncRequest request, Guid playerId) = OpenMatch(server);
        request.Known = new MatchSyncCursor { Revision = long.MaxValue };

        MatchSyncUpdate full = server.Channel.GetMatchUpdate(request);

        Assert.Null(full.BaseRevision);
        MatchSyncState state = MatchSynchronization.Apply(full, null, playerId);
        Assert.Equal(request.MatchId, state.Board.MatchId);
        Assert.Equal(playerId, state.Hand!.PlayerId);
    }

    [Fact]
    public async Task GetMatchUpdate_ValidSessionOutsideLobby_RejectsTheRead()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        (MatchSyncRequest request, _) = OpenMatch(server);
        request.SessionToken = server.Guests.Create().Token;

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.GetMatchUpdate(request));

        Assert.Equal(ServiceErrorCodes.LobbyNotMember, fault.Detail.ErrorCode);
    }

    [Fact]
    public async Task VoteToExpelMatchPlayer_RejectionAndExpulsionReachTheTargetOverNetTcp()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        ILobbyService channel = server.Channel;
        AuthenticatedSession requester = server.Guests.Create();
        AuthenticatedSession voter = server.Guests.Create();
        AuthenticatedSession target = server.Guests.Create();
        string lobby = channel.CreateLobby(true, 3, requester.Token);
        channel.JoinLobby(lobby, voter.Token);
        channel.JoinLobby(lobby, target.Token);
        foreach (AuthenticatedSession player in new[] { requester, voter, target })
        {
            channel.SetReady(lobby, true, player.Token);
        }

        LobbyPreparation preparation = channel.StartMatch(lobby, requester.Token);
        MatchSyncRequest sync = new()
        {
            LobbyCode = lobby,
            MatchId = preparation.MatchId,
            SessionToken = target.Token
        };
        MatchSyncState initial = MatchSynchronization.Apply(channel.GetMatchUpdate(sync), null, target.PlayerId);
        MatchExpulsionVoteRequest request = new()
        {
            LobbyCode = lobby,
            MatchId = preparation.MatchId,
            SessionToken = requester.Token,
            TargetPlayerId = target.PlayerId,
            ExpectedVotingRevision = initial.Board.VotingRevision,
            Decision = MatchExpulsionDecision.Request
        };
        MatchExpulsionVoteRequest response = new()
        {
            LobbyCode = request.LobbyCode,
            MatchId = request.MatchId,
            SessionToken = voter.Token,
            TargetPlayerId = request.TargetPlayerId,
            ExpectedVotingRevision = request.ExpectedVotingRevision,
            Decision = MatchExpulsionDecision.Reject
        };

        MatchExpulsionVoteSummary ballot = Assert.Single(channel.VoteToExpelMatchPlayer(request).ExpulsionVotes);
        response.BallotId = ballot.BallotId;
        channel.VoteToExpelMatchPlayer(response);
        sync.Known = MatchSynchronization.Describe(initial);
        MatchSyncState rejected = MatchSynchronization.Apply(channel.GetMatchUpdate(sync), initial, target.PlayerId);

        MatchExpulsionVoteSummary closed = Assert.Single(rejected.Board.ExpulsionVotes);
        Assert.True(closed.IsRejected);
        Assert.Equal(1, closed.VoteCount);
        Assert.Equal(1, closed.RejectedCount);
        Assert.NotNull(rejected.Hand);

        request.BallotId = closed.BallotId;
        ballot = Assert.Single(channel.VoteToExpelMatchPlayer(request).ExpulsionVotes);
        response.BallotId = ballot.BallotId;
        response.Decision = MatchExpulsionDecision.Accept;
        channel.VoteToExpelMatchPlayer(response);
        sync.Known = MatchSynchronization.Describe(rejected);
        MatchSyncState expelled = MatchSynchronization.Apply(channel.GetMatchUpdate(sync), rejected, target.PlayerId);

        Assert.Null(expelled.Hand);
        Assert.Contains(target.PlayerId, expelled.Board.ExpelledByVotePlayerIds);
        MatchBoardMove departure = Assert.Single(expelled.Board.RecentMoves);
        Assert.Equal(MatchBoardActionKind.ExpelledByVote, departure.ActionKind);
        Assert.Equal(target.PlayerId, departure.PlayerId);
    }

    [Fact]
    public void Apply_DeltaAfterExpulsion_ClearsHandAndRejectsWrongBase()
    {
        Guid self = Guid.NewGuid();
        MatchBoardSnapshot before = new() { MatchId = Guid.NewGuid(), Revision = 5 };
        MatchPrivateHand hand = new() { MatchId = before.MatchId, PlayerId = self, Revision = 5, Cards = [12] };
        MatchSyncState baseline = new(before, hand);
        MatchBoardSnapshot next = new()
        {
            MatchId = before.MatchId,
            Revision = 6,
            ExpelledPlayerIds = [self],
            ExpelledByVotePlayerIds = [self]
        };
        MatchSyncUpdate delta = MatchSynchronization.Compare(new MatchSyncState(next, null), self,
            MatchSynchronization.Describe(baseline));

        MatchSyncState received = MatchSynchronization.Apply(delta, baseline, self);

        Assert.Null(received.Hand);
        Assert.Contains(self, received.Board.ExpelledPlayerIds);
        Assert.Contains(self, received.Board.ExpelledByVotePlayerIds);
        Assert.Throws<InvalidDataException>(() => MatchSynchronization.Apply(delta, null, self));
    }

    private static (MatchSyncRequest Request, Guid PlayerId) OpenMatch(LobbyHarness server)
    {
        ILobbyService channel = server.Channel;
        AuthenticatedSession first = server.Guests.Create();
        string otherToken = server.Guests.Create().Token;
        string lobby = channel.CreateLobby(true, 2, first.Token);
        channel.JoinLobby(lobby, otherToken);
        channel.SetReady(lobby, true, first.Token);
        channel.SetReady(lobby, true, otherToken);
        LobbyPreparation preparation = channel.StartMatch(lobby, first.Token);
        return (new MatchSyncRequest
        {
            LobbyCode = lobby,
            MatchId = preparation.MatchId,
            SessionToken = first.Token
        }, first.PlayerId);
    }

    private static long Bytes<T>(T message)
    {
        using MemoryStream stream = new();
        new DataContractSerializer(typeof(T)).WriteObject(stream, message);
        return stream.Length;
    }
}
