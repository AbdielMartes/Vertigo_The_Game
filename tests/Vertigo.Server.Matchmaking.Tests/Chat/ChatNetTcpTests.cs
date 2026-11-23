using System.ServiceModel;
using System.Threading.Channels;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.Matchmaking.Tests.Lobbies;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Tests.Chat;

public sealed class ChatNetTcpTests
{
    private const int MessagesPerPlayer = 8;

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SendChat_ValidatesBothPathsAndOnlyDeliversAcceptedMessages(bool inMatch)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        ChatCallbacks hostCallbacks = new();
        ChatCallbacks guestCallbacks = new();
        using DuplexChannelFactory<ILobbyService> hostFactory = new(
            new InstanceContext(hostCallbacks), server.Binding, server.Address);
        using DuplexChannelFactory<ILobbyService> guestFactory = new(
            new InstanceContext(guestCallbacks), server.Binding, server.Address);
        using ChannelFactory<IChatService> chatFactory = CreateChatFactory(server);
        ILobbyService host = hostFactory.CreateChannel();
        ILobbyService guest = guestFactory.CreateChannel();
        IChatService hostChat = chatFactory.CreateChannel();
        IChatService guestChat = chatFactory.CreateChannel();
        var hostSession = server.Guests.Create();
        var guestSession = server.Guests.Create();

        try
        {
            string code = host.CreateLobby(true, 2, hostSession.Token);
            guest.JoinLobby(code, guestSession.Token);
            Guid? matchId = null;
            if (inMatch)
            {
                host.SetReady(code, true, hostSession.Token);
                guest.SetReady(code, true, guestSession.Token);
                matchId = host.StartMatch(code, hostSession.Token).MatchId;
            }

            ChatRoom room = new(code, matchId);
            VerifyRejections(hostChat, room, hostSession.Token);
            await VerifyDeliveryAsync(
                (hostChat, hostSession.Token, hostSession.PlayerId), room, (hostCallbacks, guestCallbacks));

            bool[] simultaneous = await Task.WhenAll(
                Task.Run(() => Send(hostChat, room, hostSession.Token, "Del anfitrión")),
                Task.Run(() => Send(guestChat, room, guestSession.Token, "De la invitada")));
            Assert.All(simultaneous, accepted => Assert.True(accepted));
            await VerifyConcurrentDeliveryAsync(hostCallbacks);
            await VerifyConcurrentDeliveryAsync(guestCallbacks);
        }
        finally
        {
            ((IClientChannel)hostChat).Abort();
            ((IClientChannel)guestChat).Abort();
            ((IClientChannel)host).Abort();
            ((IClientChannel)guest).Abort();
            chatFactory.Abort();
            hostFactory.Abort();
            guestFactory.Abort();
        }
    }

    [Theory]
    [InlineData(false, 2)]
    [InlineData(false, 3)]
    [InlineData(false, 4)]
    [InlineData(false, 5)]
    [InlineData(false, 6)]
    [InlineData(true, 2)]
    [InlineData(true, 3)]
    [InlineData(true, 4)]
    [InlineData(true, 5)]
    [InlineData(true, 6)]
    public async Task SendChat_ConcurrentPlayers_ReceiveTheSameOrderedSequence(bool inMatch, int playerCount)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using ChannelFactory<IChatService> chatFactory = CreateChatFactory(server);
        ChatPlayer[] players = Enumerable.Range(0, playerCount)
            .Select(_ => new ChatPlayer(server, chatFactory)).ToArray();

        try
        {
            ChatRoom room = StartChatRoom(players, inMatch);
            VerifyRejections(players[0].Chat, room, players[0].Session.Token);
            TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);
            Task[] senders = players.Select((player, index) =>
                SendBatchAsync((player, index), room, start.Task)).ToArray();

            start.SetResult();
            await Task.WhenAll(senders);

            (long Sequence, Guid Sender, string Text)[] history = await ReadHistoryAsync(
                players[0].Callbacks, playerCount * MessagesPerPlayer);
            Assert.Equal(Enumerable.Range(1, history.Length).Select(index => (long)index),
                history.Select(message => message.Sequence));
            foreach (ChatPlayer player in players.Skip(1))
            {
                Assert.Equal(history, await ReadHistoryAsync(player.Callbacks, history.Length));
            }

            for (int index = 0; index < players.Length; index++)
            {
                Guid sender = players[index].Session.PlayerId;
                Assert.Equal(Enumerable.Range(0, MessagesPerPlayer).Select(number => $"Mensaje {index}-{number}"),
                    history.Where(message => message.Sender == sender).Select(message => message.Text));
            }
        }
        finally
        {
            foreach (ChatPlayer player in players)
            {
                player.Dispose();
            }

            chatFactory.Abort();
        }
    }

    private static ChatRoom StartChatRoom(ChatPlayer[] players, bool inMatch)
    {
        ChatPlayer host = players[0];
        string code = host.Lobby.CreateLobby(true, players.Length, host.Session.Token);
        foreach (ChatPlayer guest in players.Skip(1))
        {
            guest.Lobby.JoinLobby(code, guest.Session.Token);
        }

        Guid? match = null;
        if (inMatch)
        {
            match = StartMatch(players, code);
        }

        return new ChatRoom(code, match);
    }

    private static Guid StartMatch(ChatPlayer[] players, string code)
    {
        foreach (ChatPlayer player in players)
        {
            player.Lobby.SetReady(code, true, player.Session.Token);
        }

        ChatPlayer host = players[0];
        return host.Lobby.StartMatch(code, host.Session.Token).MatchId;
    }

    [Fact]
    public async Task SendChat_WhenLobbyStartsMatch_ContinuesTheSameSequence()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using ChannelFactory<IChatService> factory = CreateChatFactory(server);
        using ChatPlayer host = new(server, factory);
        using ChatPlayer guest = new(server, factory);
        ChatPlayer[] players = [host, guest];
        ChatRoom lobby = StartChatRoom(players, inMatch: false);

        Assert.True(Send(host.Chat, lobby, host.Session.Token, "Listos"));
        ChatRoom match = lobby with { MatchId = StartMatch(players, lobby.Code) };
        Assert.True(Send(guest.Chat, match, guest.Session.Token, "Empezamos"));

        (long Sequence, Guid Sender, string Text)[] expected =
        [
            (1, host.Session.PlayerId, "Listos"),
            (2, guest.Session.PlayerId, "Empezamos")
        ];
        Assert.Equal(expected, await ReadHistoryAsync(host.Callbacks, expected.Length));
        Assert.Equal(expected, await ReadHistoryAsync(guest.Callbacks, expected.Length));
    }

    [Fact]
    public async Task SendMatchChat_AfterHostRestart_DeliversNewMessagesToBothReconnectedPlayers()
    {
        DirectoryInfo storage = Directory.CreateTempSubdirectory();
        try
        {
            await VerifyChatAfterHostRestartAsync(storage.FullName);
        }
        finally
        {
            storage.Delete(recursive: true);
        }
    }

    private static async Task VerifyChatAfterHostRestartAsync(string directory)
    {
        ChatRoom room;
        AuthenticatedSession first;
        AuthenticatedSession second;
        using (RecoveryDirectory journal = new(directory))
        await using (LobbyHarness before = await LobbyHarness.StartAsync(recovery: journal))
        {
            using ChannelFactory<IChatService> chatFactory = CreateChatFactory(before);
            using ChatPlayer host = new(before, chatFactory);
            using ChatPlayer guest = new(before, chatFactory);
            room = StartChatRoom([host, guest], inMatch: true);
            first = host.Session;
            second = guest.Session;
            Assert.True(Send(host.Chat, room, first.Token, "Antes del reinicio"));
            Assert.Equal("Antes del reinicio", (await guest.Callbacks.ReadAsync()).Text);
        }

        using RecoveryDirectory retained = new(directory);
        await using LobbyHarness after = await LobbyHarness.StartAsync(recovery: retained);
        using ChannelFactory<IChatService> restoredChatFactory = CreateChatFactory(after);
        using ChatPlayer restoredHost = new(after, restoredChatFactory, first);
        using ChatPlayer restoredGuest = new(after, restoredChatFactory, second);
        Reconnect(restoredHost, room);
        Reconnect(restoredGuest, room);

        Assert.True(Send(restoredGuest.Chat, room, second.Token, "Ya volví"));
        Assert.True(Send(restoredHost.Chat, room, first.Token, "Bienvenido"));

        (long Sequence, Guid Sender, string Text)[] expected =
        [
            (1, second.PlayerId, "Ya volví"),
            (2, first.PlayerId, "Bienvenido")
        ];
        Assert.Equal(expected, await ReadHistoryAsync(restoredHost.Callbacks, expected.Length));
        Assert.Equal(expected, await ReadHistoryAsync(restoredGuest.Callbacks, expected.Length));
    }

    private static void Reconnect(ChatPlayer player, ChatRoom room)
    {
        player.Lobby.ReconnectMatch(new MatchPresenceRequest
        {
            LobbyCode = room.Code, MatchId = room.MatchId!.Value, SessionToken = player.Session.Token
        });
    }

    private static ChannelFactory<IChatService> CreateChatFactory(LobbyHarness server)
    {
        UriBuilder address = new(server.Address.Uri) { Path = "/ChatService" };
        return new ChannelFactory<IChatService>(server.Binding, new EndpointAddress(address.Uri));
    }

    private static async Task SendBatchAsync((ChatPlayer Player, int Index) sender, ChatRoom room, Task start)
    {
        await start;
        for (int number = 0; number < MessagesPerPlayer; number++)
        {
            bool accepted = Send(sender.Player.Chat, room, sender.Player.Session.Token, $"Mensaje {sender.Index}-{number}");
            Assert.True(accepted);
        }
    }

    private static async Task<(long Sequence, Guid Sender, string Text)[]> ReadHistoryAsync(
        ChatCallbacks callbacks, int count)
    {
        (long Sequence, Guid Sender, string Text)[] history = new (long, Guid, string)[count];
        for (int index = 0; index < count; index++)
        {
            LobbyChatMessage message = await callbacks.ReadAsync();
            history[index] = (message.Sequence, message.SenderId, message.Text);
        }

        Assert.False(callbacks.HasPendingMessage);
        return history;
    }

    private static void VerifyRejections(IChatService chat, ChatRoom room, string token)
    {
        if (room.MatchId.HasValue)
        {
            FaultException<ServiceFault> missingRequest = Assert.Throws<FaultException<ServiceFault>>(
                () => chat.SendMatchMessage(null!));
            Assert.Equal(ServiceErrorCodes.InvalidData, missingRequest.Detail.ErrorCode);
        }

        string?[] invalid = [null, "", "  \t\r\n", "hola\nmundo", "hola\rmundo", "hola\u0085mundo",
            "hola\u2028mundo", "hola\u2029mundo", new string('a', 31), string.Concat(Enumerable.Repeat("😀", 15)) + "a"];
        foreach (string? text in invalid)
        {
            FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
                () => Send(chat, room, token, text!));
            Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
        }

        FaultException<ServiceFault> blocked = Assert.Throws<FaultException<ServiceFault>>(
            () => Send(chat, room, token, "¡PÉNDEJ0!"));
        Assert.Equal(ServiceErrorCodes.ChatLanguageBlocked, blocked.Detail.ErrorCode);
        Assert.Equal(ServiceMessageKeys.ChatLanguageBlocked, blocked.Detail.MessageKey);
    }

    private static async Task VerifyDeliveryAsync((IChatService Chat, string Token, Guid PlayerId) sender, ChatRoom room,
        (ChatCallbacks Host, ChatCallbacks Guest) recipients)
    {
        string[] valid = ["a", new string('a', 30), "¡Sí, mañana! 🏙️", "<b>hola</b>", "O'Connor -- ;",
            "Bien jugado, diputado.", string.Concat(Enumerable.Repeat("😀", 15))];
        foreach (string text in valid)
        {
            Assert.True(Send(sender.Chat, room, sender.Token, text));
        }

        foreach (string text in valid)
        {
            LobbyChatMessage echo = await recipients.Host.ReadAsync();
            LobbyChatMessage received = await recipients.Guest.ReadAsync();
            Assert.Equal(text, echo.Text);
            Assert.Equal(text, received.Text);
            Assert.Equal(sender.PlayerId, received.SenderId);
            Assert.Equal(room.MatchId, received.MatchId);
            Assert.Equal(room.Code, received.LobbyCode);
            Assert.Equal(echo.SentAt, received.SentAt);
        }
    }

    private static async Task VerifyConcurrentDeliveryAsync(ChatCallbacks callbacks)
    {
        LobbyChatMessage first = await callbacks.ReadAsync();
        LobbyChatMessage second = await callbacks.ReadAsync();
        Assert.Equal(new[] { "De la invitada", "Del anfitrión" },
            new[] { first.Text, second.Text }.Order(StringComparer.Ordinal));
        Assert.NotEqual(first.SenderId, second.SenderId);
        Assert.False(callbacks.HasPendingMessage);
    }

    private static bool Send(IChatService chat, ChatRoom room, string token, string text)
    {
        return room.MatchId is Guid matchId
            ? chat.SendMatchMessage(new MatchChatRequest
            {
                LobbyCode = room.Code, MatchId = matchId, SessionToken = token, Text = text
            })
            : chat.SendMessage(room.Code, text, token);
    }

    private sealed record ChatRoom(string Code, Guid? MatchId);

    private sealed class ChatPlayer : IDisposable
    {
        private readonly DuplexChannelFactory<ILobbyService> _factory;

        public ChatPlayer(LobbyHarness server, ChannelFactory<IChatService> chatFactory,
            AuthenticatedSession? session = null)
        {
            _factory = new DuplexChannelFactory<ILobbyService>(new InstanceContext(Callbacks), server.Binding,
                server.Address);
            Lobby = _factory.CreateChannel();
            Chat = chatFactory.CreateChannel();
            Session = session ?? server.Guests.Create();
        }

        public ChatCallbacks Callbacks { get; } = new();
        public ILobbyService Lobby { get; }
        public IChatService Chat { get; }
        public AuthenticatedSession Session { get; }

        public void Dispose()
        {
            ((IClientChannel)Chat).Abort();
            ((IClientChannel)Lobby).Abort();
            _factory.Abort();
        }
    }

    private sealed class ChatCallbacks : ILobbyCallback
    {
        private readonly Channel<LobbyChatMessage> _messages = Channel.CreateUnbounded<LobbyChatMessage>();

        public bool HasPendingMessage => _messages.Reader.TryPeek(out _);

        public void Notify(LobbyNotification notification)
        {
            if (notification.ChatMessage is { } message)
            {
                _messages.Writer.TryWrite(message);
            }
        }

        public async Task<LobbyChatMessage> ReadAsync()
        {
            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));
            return await _messages.Reader.ReadAsync(timeout.Token);
        }
    }
}
