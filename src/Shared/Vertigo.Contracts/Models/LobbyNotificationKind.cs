using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum LobbyNotificationKind
{
    [EnumMember]
    None,

    [EnumMember]
    PlayerJoined,

    [EnumMember]
    PlayerLeft,

    [EnumMember]
    MessageReceived,

    [EnumMember]
    PreparationChanged,

    [EnumMember]
    MatchBoardUpdated
}
