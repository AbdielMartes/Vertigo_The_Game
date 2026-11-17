using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyStateTests
{
    [Fact]
    public void Remove_Host_TransfersHostToLongestPresentPlayer()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "a");
        Add(lobby, second, "b");
        Add(lobby, third, "c");

        Assert.True(lobby.Remove(host));

        Assert.Equal(second, lobby.HostPlayerId);
        Assert.False(lobby.Contains(host));
        Assert.True(lobby.Contains(third));
    }

    [Fact]
    public void RemoveSession_RemovesOnlyMembersOfThatChannel()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "first");
        Add(lobby, second, "second");
        Add(lobby, third, "third");

        List<Guid> removed = lobby.RemoveSession("second");

        Assert.Equal(new[] { second }, removed);
        Assert.True(lobby.Contains(host));
        Assert.True(lobby.Contains(third));
    }

    [Fact]
    public void CancelInvitation_LeavesOtherInvitationsIntact()
    {
        LobbyState lobby = CreateLobby(Guid.NewGuid());
        Guid first = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        lobby.Invite(first, Guid.NewGuid());
        lobby.Invite(second, Guid.NewGuid());

        Assert.True(lobby.CancelInvitation(first));

        Assert.False(lobby.IsInvited(first));
        Assert.True(lobby.IsInvited(second));
    }

    [Fact]
    public void SetReady_NonMember_ReturnsFalse()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");

        Assert.False(lobby.SetReady(Guid.NewGuid(), true));
        Assert.True(lobby.SetReady(host, true));
        Assert.Contains(host, lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void CanStartMatch_RequiresTwoPlayersAllReadyAndHost()
    {
        Guid host = Guid.NewGuid();
        Guid other = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        Assert.False(lobby.CanStartMatch(host));
        Add(lobby, other, "other");
        lobby.SetReady(host, true);
        Assert.False(lobby.CanStartMatch(host));
        lobby.SetReady(other, true);

        Assert.True(lobby.CanStartMatch(host));
        Assert.False(lobby.CanStartMatch(other));
        lobby.StartMatch(Guid.NewGuid());
        Assert.False(lobby.CanStartMatch(host));
        Assert.True(lobby.ToPreparation().IsInGame);
    }

    [Fact]
    public void Join_ResetsReadyVotes()
    {
        Guid host = Guid.NewGuid();
        Guid second = Guid.NewGuid();
        Guid third = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "a");
        Add(lobby, second, "b");
        lobby.SetReady(host, true);
        lobby.SetReady(second, true);

        Add(lobby, third, "c");

        Assert.Empty(lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void Remove_ReadyPlayer_ExcludesDepartedPlayerFromPreparation()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        lobby.SetReady(host, true);

        lobby.Remove(host);

        Assert.Empty(lobby.ToPreparation().ReadyPlayerIds);
    }

    [Fact]
    public void ToPreparation_ConsecutiveChanges_RevisionIncreases()
    {
        Guid host = Guid.NewGuid();
        Guid visitor = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        long initial = lobby.ToPreparation().Revision;
        Add(lobby, host, "first");
        long entered = lobby.ToPreparation().Revision;
        Add(lobby, visitor, "second");
        long joined = lobby.ToPreparation().Revision;
        lobby.SetReady(host, true);
        long ready = lobby.ToPreparation().Revision;
        lobby.Remove(host);
        long transferred = lobby.ToPreparation().Revision;

        Assert.True(initial < entered);
        Assert.True(entered < joined);
        Assert.True(joined < ready);
        Assert.True(ready < transferred);
    }

    [Fact]
    public void SetReady_SameValue_DoesNotAdvanceRevision()
    {
        Guid host = Guid.NewGuid();
        LobbyState lobby = CreateLobby(host);
        Add(lobby, host, "host");
        lobby.SetReady(host, true);
        long revision = lobby.Revision;

        Assert.True(lobby.SetReady(host, true));
        Assert.Equal(revision, lobby.Revision);
    }

    private static LobbyState CreateLobby(Guid host)
    {
        return new LobbyState(new LobbySettings("ABCDEF", host, true, 6, DateTimeOffset.UtcNow));
    }

    private static void Add(LobbyState lobby, Guid playerId, string channel)
    {
        lobby.AddOrReplace(new LobbyConnection(
            new LobbyMember { PlayerId = playerId, Username = channel }, new CallbackStub(), channel));
    }

    private sealed class CallbackStub : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember player) { }
        public void PlayerLeft(Guid playerId) { }
        public void MessageReceived(LobbyChatMessage message) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
        public void MatchBoardUpdated(MatchBoardChanged change) { }
    }
}
