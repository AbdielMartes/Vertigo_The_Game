using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchLeaveRequest
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember]
    public Guid MatchId { get; set; }
}
