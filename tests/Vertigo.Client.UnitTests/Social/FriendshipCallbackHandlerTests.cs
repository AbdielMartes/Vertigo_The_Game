using System.Reflection;
using System.ServiceModel;
using System.Runtime.Serialization;
using Vertigo.Client.Networking.Callbacks;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UnitTests.Social;

public sealed class FriendshipCallbackHandlerTests
{
    private readonly IFriendshipCallback _callback;
    private readonly List<object> _received = new();

    public FriendshipCallbackHandlerTests()
    {
        FriendshipCallbackHandler handler = new();
        handler.FriendRequestReceived += CapturePayload;
        handler.FriendshipEstablished += CapturePayload;
        handler.LobbyInvitationReceived += CapturePayload;
        handler.LobbyInvitationWithdrawn += CapturePayload;
        _callback = handler;
    }

    [Fact]
    public void Notify_SerializedFriendRequest_RaisesFriendRequestReceived()
    {
        Guid senderId = Guid.NewGuid();
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.FriendRequestReceived,
            FriendRequest = new FriendshipInvitationEntry { SenderId = senderId }
        };

        _callback.Notify(RoundTrip(notification));

        object payload = Assert.Single(_received);
        FriendshipInvitationEntry request = Assert.IsType<FriendshipInvitationEntry>(payload);
        Assert.Equal(senderId, request.SenderId);
    }

    [Fact]
    public void Notify_SerializedFriendship_RaisesFriendshipEstablished()
    {
        Guid playerId = Guid.NewGuid();
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.FriendshipEstablished,
            Friend = new FriendEntry { PlayerId = playerId }
        };

        _callback.Notify(RoundTrip(notification));

        object payload = Assert.Single(_received);
        FriendEntry friend = Assert.IsType<FriendEntry>(payload);
        Assert.Equal(playerId, friend.PlayerId);
    }

    [Fact]
    public void Notify_SerializedLobbyInvitation_RaisesLobbyInvitationReceived()
    {
        const string LobbyCode = "ABC123";
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.LobbyInvitationReceived,
            LobbyInvitation = new LobbyInvitationEntry { LobbyCode = LobbyCode }
        };

        _callback.Notify(RoundTrip(notification));

        object payload = Assert.Single(_received);
        LobbyInvitationEntry invitation = Assert.IsType<LobbyInvitationEntry>(payload);
        Assert.Equal(LobbyCode, invitation.LobbyCode);
    }

    [Fact]
    public void Notify_SerializedLobbyWithdrawal_RaisesLobbyInvitationWithdrawn()
    {
        const string LobbyCode = "ABC123";
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.LobbyInvitationWithdrawn,
            WithdrawnLobbyCode = LobbyCode
        };

        _callback.Notify(RoundTrip(notification));

        object payload = Assert.Single(_received);
        Assert.Equal(LobbyCode, Assert.IsType<string>(payload));
    }

    [Fact]
    public void Notify_ConflictingPayloads_RaisesNoEvents()
    {
        FriendshipNotification notification = new()
        {
            Kind = FriendshipNotificationKind.FriendshipEstablished,
            Friend = new FriendEntry(),
            LobbyInvitation = new LobbyInvitationEntry()
        };

        _callback.Notify(RoundTrip(notification));

        Assert.Empty(_received);
    }

    [Fact]
    public void FriendshipCallback_Contract_DeclaresOneTypedOneWayOperation()
    {
        MethodInfo method = Assert.Single(typeof(IFriendshipCallback).GetMethods());

        Assert.Equal(nameof(IFriendshipCallback.Notify), method.Name);
        Assert.True(method.GetCustomAttribute<OperationContractAttribute>()?.IsOneWay);
        Assert.Equal(typeof(void), method.ReturnType);
        Assert.Equal(typeof(FriendshipNotification), Assert.Single(method.GetParameters()).ParameterType);
    }

    [Theory]
    [InlineData(FriendshipNotificationKind.None, null)]
    [InlineData(FriendshipNotificationKind.FriendRequestReceived, null)]
    [InlineData(FriendshipNotificationKind.LobbyInvitationWithdrawn, " ")]
    public void Notify_MissingOrInvalidPayload_RaisesNoEvents(FriendshipNotificationKind kind, string? lobbyCode)
    {
        FriendshipNotification notification = new() { Kind = kind, WithdrawnLobbyCode = lobbyCode };

        _callback.Notify(RoundTrip(notification));

        Assert.Empty(_received);
    }

    private void CapturePayload(object? sender, object payload)
    {
        _received.Add(payload);
    }

    private static FriendshipNotification RoundTrip(FriendshipNotification notification)
    {
        DataContractSerializer serializer = new(typeof(FriendshipNotification));
        using MemoryStream stream = new();
        serializer.WriteObject(stream, notification);
        stream.Position = 0;

        return Assert.IsType<FriendshipNotification>(serializer.ReadObject(stream));
    }
}
