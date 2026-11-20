using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Tests.Chat;

public sealed class ChatServiceTests
{
    [Theory]
    [InlineData(30, true)]
    [InlineData(31, false)]
    public void SendMatchMessage_ValidatesLengthBeforePublishing(int length, bool accepted)
    {
        RecordingLobbyDirectory directory = new();
        StubSessionResolver sessions = new();
        ChatService service = CreateService(directory, sessions);
        MatchChatRequest request = new()
        {
            LobbyCode = "ABC234", MatchId = Guid.NewGuid(), SessionToken = "session", Text = new string('a', length)
        };

        if (accepted)
        {
            Assert.True(service.SendMatchMessage(request));
            LobbyChatMessage message = Assert.IsType<LobbyChatMessage>(directory.Message);
            Assert.Equal(request.MatchId, message.MatchId);
            Assert.Equal(request.Text, message.Text);
            Assert.Equal(sessions.Identity.PlayerId, message.SenderId);
        }
        else
        {
            var fault = Assert.Throws<FaultException<ServiceFault>>(() => service.SendMatchMessage(request));
            Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
            Assert.Null(directory.Message);
        }
    }

    [Fact]
    public void SendMessage_WaitingRoomRetainsFiveHundredCharacterLimitAndNoMatchId()
    {
        RecordingLobbyDirectory directory = new();
        ChatService service = CreateService(directory, new StubSessionResolver());
        string text = new('a', 500);

        Assert.True(service.SendMessage("ABC234", text, "session"));
        LobbyChatMessage message = Assert.IsType<LobbyChatMessage>(directory.Message);
        Assert.Equal(text, message.Text);
        Assert.Null(message.MatchId);

        Assert.Throws<FaultException<ServiceFault>>(() => service.SendMessage("ABC234", text + "a", "session"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BlockedLanguage_IsRejectedWithoutPublishing(bool inMatch)
    {
        RecordingLobbyDirectory directory = new();
        ChatService service = CreateService(directory, new StubSessionResolver());

        FaultException<ServiceFault> fault =
            Assert.Throws<FaultException<ServiceFault>>(() => Send(inMatch, service, "¡PÉNDEJ0!"));

        Assert.Equal(ServiceErrorCodes.ChatLanguageBlocked, fault.Detail.ErrorCode);
        Assert.Equal(ServiceMessageKeys.ChatLanguageBlocked, fault.Detail.MessageKey);
        Assert.Null(directory.Message);

        Assert.True(Send(inMatch, service, "Bien jugado, diputado."));
        Assert.Equal("Bien jugado, diputado.", directory.Message!.Text);
    }

    private static bool Send(bool inMatch, ChatService service, string text)
    {
        return inMatch ? service.SendMatchMessage(new MatchChatRequest
        {
            LobbyCode = "ABC234", MatchId = Guid.NewGuid(), SessionToken = "session", Text = text
        }) : service.SendMessage("ABC234", text, "session");
    }

    private static ChatService CreateService(RecordingLobbyDirectory directory, StubSessionResolver sessions)
    {
        ChatServiceDependencies dependencies = new(sessions, TimeProvider.System, new ChatLanguageFilter());
        return new ChatService(directory, dependencies);
    }

    private sealed class StubSessionResolver : ISessionIdentityResolver
    {
        public SessionIdentity Identity { get; } = new(Guid.NewGuid(), "Ana", true);

        public SessionIdentity? Resolve(string token) => throw new NotSupportedException();

        public SessionIdentity Require(string token) => Identity;

        public SessionIdentity RequireRegistered(string token) => throw new NotSupportedException();
    }

    private sealed class RecordingLobbyDirectory : ILobbyDirectory
    {
        public LobbyChatMessage? Message { get; private set; }

        public bool PublishChatMessage(LobbyChatMessage message)
        {
            Message = message;
            return true;
        }

        public LobbyOverview? FindLobby(string lobbyCode) => throw new NotSupportedException();

        public bool IsInAnyLobby(Guid playerId) => throw new NotSupportedException();

        public bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId) =>
            throw new NotSupportedException();

        public bool CancelInvitation(string lobbyCode, Guid targetPlayerId) => throw new NotSupportedException();

        public List<LobbyOverview> FindLobbiesInviting(Guid playerId) => throw new NotSupportedException();
    }
}
