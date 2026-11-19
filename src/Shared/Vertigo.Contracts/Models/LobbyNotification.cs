using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyNotification
{
    [DataMember]
    public LobbyNotificationKind Kind { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public LobbyMember? JoinedPlayer { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? LeftPlayerId { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public LobbyChatMessage? ChatMessage { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public LobbyPreparation? Preparation { get; init; }

    [DataMember(EmitDefaultValue = false)]
    public MatchBoardChanged? BoardChange { get; init; }
}
