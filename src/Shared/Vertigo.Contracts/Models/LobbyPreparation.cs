using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyPreparation
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public Guid HostPlayerId { get; set; }

    [DataMember]
    public bool IsInGame { get; set; }

    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public List<Guid> ReadyPlayerIds { get; set; } = new();

    [DataMember]
    public List<Guid> DisconnectedPlayerIds { get; set; } = new();

    [DataMember]
    public bool IsSuspended { get; set; }
}
