using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class FriendshipNotification
{
    [DataMember]
    public FriendshipNotificationKind Kind { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public FriendshipInvitationEntry? FriendRequest { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public FriendEntry? Friend { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public LobbyInvitationEntry? LobbyInvitation { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public string? WithdrawnLobbyCode { get; init; }
}
