using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;
using Vertigo.Server.Matchmaking.Lobbies.Match;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyMatchActivationTests
{
    [Fact]
    public void Activate_ValidLobby_SetsMatchIdAndInGame()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter());
        long previousRevision = lobby.Revision;

        activation.Activate(lobby);

        Assert.True(lobby.ToPreparation().IsInGame);
        Assert.NotEqual(Guid.Empty, lobby.MatchId);
        Assert.True(lobby.Revision > previousRevision);
    }

    [Fact]
    public void Activate_StarterFails_KeepsWaitingAndRevision()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter(shouldThrow: true));
        long revision = lobby.Revision;

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));

        Assert.Equal(Guid.Empty, lobby.MatchId);
        Assert.False(lobby.ToPreparation().IsInGame);
        Assert.Equal(revision, lobby.Revision);
    }

    [Fact]
    public void Activate_DuplicateAttempt_DoesNotCreateAgain()
    {
        LobbyState lobby = CreateReadyLobby();
        FakeStarter starter = new();
        LobbyMatchActivation activation = new(starter);
        activation.Activate(lobby);

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));
        Assert.Equal(1, starter.Count);
    }

    [Fact]
    public void Activate_StarterCommitsThenThrows_RetryUsesSameMatchAndPreservesRevisionUntilSuccess()
    {
        LobbyState lobby = CreateReadyLobby();
        CommitThenThrowStarter starter = new();
        LobbyMatchActivation activation = new(starter);
        long before = lobby.Revision;

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));

        Assert.Equal(Guid.Empty, lobby.MatchId);
        Assert.False(lobby.ToPreparation().IsInGame);
        Assert.Equal(before, lobby.Revision);

        activation.Activate(lobby);

        Assert.Equal(starter.CreatedMatchId, lobby.MatchId);
        Assert.True(lobby.ToPreparation().IsInGame);
        Assert.Equal(before + 1, lobby.Revision);
        Assert.Equal(2, starter.CallCount);
        Assert.Equal(1, starter.CreatedCount);
    }

    private sealed class CommitThenThrowStarter : IMatchSessionStarter
    {
        private readonly Dictionary<Guid, MatchSessionSnapshot> _registered = new();
        private bool _throwAfterCommit = true;

        public int CallCount { get; private set; }
        public int CreatedCount => _registered.Count;
        public Guid CreatedMatchId => Assert.Single(_registered.Values).MatchId;

        public MatchSessionSnapshot Start(MatchStartRequest request)
        {
            CallCount++;
            if (!_registered.TryGetValue(request.LobbyInstanceId, out MatchSessionSnapshot? session))
            {
                session = new MatchSessionSnapshot(Guid.NewGuid(), request.LobbyInstanceId,
                    request.LobbyCode, request.HostPlayerId, request.PlayerIds, DateTime.UtcNow);
                _registered.Add(request.LobbyInstanceId, session);
            }

            if (_throwAfterCommit)
            {
                _throwAfterCommit = false;
                throw new InvalidOperationException("Simulated failure after registration.");
            }

            return session;
        }
    }

    [Fact]
    public void Activate_InconsistentGameResult_KeepsWaiting()
    {
        LobbyState lobby = CreateReadyLobby();
        LobbyMatchActivation activation = new(new FakeStarter(wrongLobby: true));

        Assert.Throws<InvalidOperationException>(() => activation.Activate(lobby));
        Assert.False(lobby.ToPreparation().IsInGame);
    }

    private static LobbyState CreateReadyLobby()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = new(new LobbySettings("ABCDEF", host, false, 4, DateTimeOffset.UtcNow));
        lobby.AddOrReplace(new LobbyConnection(new LobbyMember { PlayerId = host }, new StubCallback(), "a"));
        lobby.AddOrReplace(new LobbyConnection(new LobbyMember { PlayerId = other }, new StubCallback(), "b"));
        lobby.SetReady(host, true);
        lobby.SetReady(other, true);
        return lobby;
    }

    private sealed class FakeStarter : IMatchSessionStarter
    {
        private readonly bool _shouldThrow;
        private readonly bool _wrongLobby;
        public int Count { get; private set; }

        public FakeStarter(bool shouldThrow = false, bool wrongLobby = false)
        {
            _shouldThrow = shouldThrow;
            _wrongLobby = wrongLobby;
        }

        public MatchSessionSnapshot Start(MatchStartRequest request)
        {
            Count++;
            if (_shouldThrow)
            {
                throw new InvalidOperationException("Simulated game failure.");
            }

            return new MatchSessionSnapshot(Guid.NewGuid(), request.LobbyInstanceId,
                _wrongLobby ? "WRONG" : request.LobbyCode,
                request.HostPlayerId, request.PlayerIds, DateTime.UtcNow);
        }
    }

    private sealed class StubCallback : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember member) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
        public void MatchBoardUpdated(MatchBoardChanged change) { }
    }
}
