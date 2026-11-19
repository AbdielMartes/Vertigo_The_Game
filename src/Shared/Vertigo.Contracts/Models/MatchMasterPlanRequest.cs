using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchMasterPlanRequest
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember]
    public List<int> CardNumbers { get; set; } = new();

    [DataMember]
    public int PilePosition { get; set; }

    [DataMember]
    public long ExpectedRevision { get; set; }
}
