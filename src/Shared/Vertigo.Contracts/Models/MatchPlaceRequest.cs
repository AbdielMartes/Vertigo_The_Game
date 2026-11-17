using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchPlaceRequest
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember]
    public int CardNumber { get; set; }

    [DataMember]
    public int PilePosition { get; set; }

    [DataMember]
    public long ExpectedRevision { get; set; }
}
