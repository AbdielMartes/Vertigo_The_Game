using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyMembershipTests
{
    [Fact]
    public void Contains_WaitingParticipant_ReturnsTrue()
    {
        Guid playerId = Guid.NewGuid();
        LobbyState lobby = CreateLobby(playerId);

        bool member = lobby.Contains(playerId);

        Assert.True(member);
    }

    [Fact]
    public void DisconnectSession_InGame_RetainsMembershipForResume()
    {
        Guid playerId = Guid.NewGuid();
        LobbyState lobby = CreateLobby(playerId);
        lobby.StartMatch(Guid.NewGuid());

        lobby.DisconnectSession("session-1");

        Assert.True(lobby.Contains(playerId));
        Assert.True(lobby.ToPreparation().IsInGame);
    }

    [Fact]
    public void Remove_WaitingParticipant_ReleasesMembership()
    {
        Guid playerId = Guid.NewGuid();
        LobbyState lobby = CreateLobby(playerId);

        lobby.Remove(playerId);

        Assert.False(lobby.Contains(playerId));
    }

    private static LobbyState CreateLobby(Guid playerId)
    {
        LobbyState lobby = new(new LobbySettings("ABC234", playerId, true, 4, DateTimeOffset.UtcNow));
        LobbyMember member = new() { PlayerId = playerId, Username = "test" };
        lobby.AddOrReplace(new LobbyConnection(member, new CallbackStub(), "session-1"));
        return lobby;
    }

    private sealed class CallbackStub : ILobbyCallback
    {
        public void PlayerJoined(LobbyMember player) { }
        public void PlayerLeft(Guid playerId) { }
        public void PreparationChanged(LobbyPreparation preparation) { }
        public void MatchBoardUpdated(MatchBoardChanged change) { }
        public void MessageReceived(LobbyChatMessage message) { }
    }
}
