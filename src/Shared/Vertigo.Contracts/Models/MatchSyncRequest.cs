using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchSyncRequest
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember(EmitDefaultValue = false)]
    public MatchSyncCursor? Known { get; set; }
}
