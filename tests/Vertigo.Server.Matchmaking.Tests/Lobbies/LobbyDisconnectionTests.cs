using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyDisconnectionTests
{
    [Fact]
    public void DisconnectSession_InGame_PreservesParticipantAndHost()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "host-channel");
        Add(lobby, other, "other-channel");
        lobby.StartMatch(Guid.NewGuid());
        long revision = lobby.Revision;

        List<Guid> disconnected = lobby.DisconnectSession("host-channel");

        Assert.Equal(new[] { host }, disconnected);
        Assert.Equal(host, lobby.HostPlayerId);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(other));
        Assert.Equal(revision + 1, lobby.Revision);
        Assert.Contains(host, lobby.ToPreparation().DisconnectedPlayerIds);
        Assert.False(lobby.ToPreparation().IsSuspended);
    }

    [Fact]
    public void DisconnectSession_AllMembers_SetsSuspendedFlag()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "a");
        Add(lobby, other, "b");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("a");
        lobby.DisconnectSession("b");

        Assert.True(lobby.ToPreparation().IsSuspended);
        Assert.Equal(2, lobby.ToPreparation().DisconnectedPlayerIds.Count);
    }

    [Fact]
    public void AddOrReplace_LateCloseOfOldChannel_DoesNotDisconnectNewChannel()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "old-channel");
        Add(lobby, other, "other-channel");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("old-channel");
        Add(lobby, host, "new-channel");
        long revision = lobby.Revision;

        List<Guid> stale = lobby.DisconnectSession("old-channel");

        Assert.Empty(stale);
        Assert.Equal(revision, lobby.Revision);
        Assert.DoesNotContain(host, lobby.ToPreparation().DisconnectedPlayerIds);
    }

    [Fact]
    public void DisconnectSession_DuplicateNotification_DoesNotAdvanceRevision()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "channel");
        lobby.StartMatch(Guid.NewGuid());
        lobby.DisconnectSession("channel");
        long revision = lobby.Revision;

        Assert.Empty(lobby.DisconnectSession("channel"));
        Assert.Equal(revision, lobby.Revision);
    }

    [Fact]
    public void DisconnectSession_ReplacedChannel_DoesNotAlterExistingMembers()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = NewLobby(host);
        Add(lobby, host, "first");
        Add(lobby, other, "second");
        lobby.StartMatch(Guid.NewGuid());
        Add(lobby, host, "replacement");
        long revision = lobby.Revision;

        Assert.Empty(lobby.DisconnectSession("first"));
        Assert.Equal(revision, lobby.Revision);
        Assert.Equal(host, lobby.HostPlayerId);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(other));
    }

    private static LobbyState NewLobby(Guid host)
    {
        return new LobbyState(new LobbySettings("ABCDEF", host, true, 6, DateTimeOffset.UtcNow));
    }

    private static void Add(LobbyState lobby, Guid id, string channel)
    {
        LobbyMember member = new() { PlayerId = id, Username = channel };
        lobby.AddOrReplace(new LobbyConnection(member, new CallbackStub(), channel));
    }

    private sealed class CallbackStub : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember member) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
        public void MatchBoardUpdated(MatchBoardChanged change) { }
    }
}
