using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Lobbies;

public sealed class LobbyCallbackHandlerTests
{
    private const string LobbyCode = "ABC123";
    private const string ChatText = "Hello";
    private const long BoardRevision = 6;

    private readonly List<object> _received = new();
    private readonly ILobbyCallback _callback;

    public LobbyCallbackHandlerTests()
    {
        LobbyCallbackHandler handler = new();
        handler.PlayerJoined += CapturePayload;
        handler.PlayerLeft += CapturePlayerLeft;
        handler.MessageReceived += CapturePayload;
        handler.PreparationChanged += CapturePayload;
        handler.MatchBoardUpdated += CapturePayload;
        _callback = handler;
    }

    [Fact]
    public void Notify_SerializedNotifications_RaisesExpectedEventsInOrder()
    {
        Guid playerId = Guid.NewGuid();
        Guid matchId = Guid.NewGuid();
        LobbyNotification[] notifications = CreateNotifications(playerId, matchId);

        foreach (LobbyNotification notification in notifications)
        {
            _callback.Notify(RoundTrip(notification));
        }

        Assert.Equal(notifications.Length, _received.Count);
        LobbyMember joined = Assert.IsType<LobbyMember>(_received[0]);
        Guid leftPlayerId = Assert.IsType<Guid>(_received[1]);
        LobbyChatMessage chat = Assert.IsType<LobbyChatMessage>(_received[2]);
        LobbyPreparation preparation = Assert.IsType<LobbyPreparation>(_received[3]);
        MatchBoardChanged boardChange = Assert.IsType<MatchBoardChanged>(_received[4]);
        Assert.Equal(playerId, joined.PlayerId);
        Assert.Equal(playerId, leftPlayerId);
        Assert.Equal(ChatText, chat.Text);
        Assert.Equal(LobbyCode, preparation.LobbyCode);
        Assert.Equal(matchId, boardChange.MatchId);
        Assert.Equal(BoardRevision, boardChange.Revision);
    }

    [Fact]
    public void Notify_PayloadDoesNotMatchKind_RaisesNoEvents()
    {
        LobbyNotification notification = new()
        {
            Kind = LobbyNotificationKind.MatchBoardUpdated,
            Preparation = new LobbyPreparation { LobbyCode = LobbyCode }
        };

        _callback.Notify(RoundTrip(notification));

        Assert.Empty(_received);
    }

    [Fact]
    public void LobbyCallback_Contract_DeclaresOneTypedOneWayOperation()
    {
        MethodInfo method = Assert.Single(typeof(ILobbyCallback).GetMethods());

        Assert.Equal(nameof(ILobbyCallback.Notify), method.Name);
        Assert.True(method.GetCustomAttribute<OperationContractAttribute>()?.IsOneWay);
        Assert.Equal(typeof(void), method.ReturnType);
        Assert.Equal(typeof(LobbyNotification), Assert.Single(method.GetParameters()).ParameterType);
    }

    [Theory]
    [InlineData(LobbyNotificationKind.None, null)]
    [InlineData(LobbyNotificationKind.PreparationChanged, null)]
    [InlineData(LobbyNotificationKind.PlayerLeft, "00000000-0000-0000-0000-000000000000")]
    public void Notify_MissingOrInvalidPayload_RaisesNoEvents(LobbyNotificationKind kind, string? playerId)
    {
        LobbyNotification notification = new()
        {
            Kind = kind,
            LeftPlayerId = playerId is null ? null : Guid.Parse(playerId)
        };

        _callback.Notify(RoundTrip(notification));

        Assert.Empty(_received);
    }

    private void CapturePayload(object? sender, object payload)
    {
        _received.Add(payload);
    }

    private void CapturePlayerLeft(object? sender, Guid playerId)
    {
        _received.Add(playerId);
    }

    private static LobbyNotification[] CreateNotifications(Guid playerId, Guid matchId)
    {
        return new LobbyNotification[]
        {
            new()
            {
                Kind = LobbyNotificationKind.PlayerJoined,
                JoinedPlayer = new LobbyMember { PlayerId = playerId }
            },
            new()
            {
                Kind = LobbyNotificationKind.PlayerLeft,
                LeftPlayerId = playerId
            },
            new()
            {
                Kind = LobbyNotificationKind.MessageReceived,
                ChatMessage = new LobbyChatMessage { LobbyCode = LobbyCode, Text = ChatText }
            },
            new()
            {
                Kind = LobbyNotificationKind.PreparationChanged,
                Preparation = new LobbyPreparation { LobbyCode = LobbyCode }
            },
            new()
            {
                Kind = LobbyNotificationKind.MatchBoardUpdated,
                BoardChange = new MatchBoardChanged
                {
                    LobbyCode = LobbyCode,
                    MatchId = matchId,
                    PlayerUsername = "player",
                    Revision = BoardRevision
                }
            }
        };
    }

    private static LobbyNotification RoundTrip(LobbyNotification notification)
    {
        DataContractSerializer serializer = new(typeof(LobbyNotification));
        using MemoryStream stream = new();
        serializer.WriteObject(stream, notification);
        stream.Position = 0;

        return Assert.IsType<LobbyNotification>(serializer.ReadObject(stream));
    }
}
