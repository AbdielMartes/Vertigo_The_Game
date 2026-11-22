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
    public void Apply_DeltaAfterExpulsion_ClearsHandAndRejectsWrongBase()
    {
        Guid self = Guid.NewGuid();
        MatchBoardSnapshot before = new() { MatchId = Guid.NewGuid(), Revision = 5 };
        MatchPrivateHand hand = new() { MatchId = before.MatchId, PlayerId = self, Revision = 5, Cards = [12] };
        MatchSyncState baseline = new(before, hand);
        MatchBoardSnapshot next = new() { MatchId = before.MatchId, Revision = 6, ExpelledPlayerIds = [self] };
        MatchSyncUpdate delta = MatchSynchronization.Compare(new MatchSyncState(next, null), self,
            MatchSynchronization.Describe(baseline));

        MatchSyncState received = MatchSynchronization.Apply(delta, baseline, self);

        Assert.Null(received.Hand);
        Assert.Contains(self, received.Board.ExpelledPlayerIds);
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
