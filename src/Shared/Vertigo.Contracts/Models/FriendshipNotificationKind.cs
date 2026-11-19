using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum FriendshipNotificationKind
{
    [EnumMember]
    None,

    [EnumMember]
    FriendRequestReceived,

    [EnumMember]
    FriendshipEstablished,

    [EnumMember]
    LobbyInvitationReceived,

    [EnumMember]
    LobbyInvitationWithdrawn
}
