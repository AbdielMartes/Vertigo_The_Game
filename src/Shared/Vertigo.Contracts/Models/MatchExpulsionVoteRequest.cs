using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchExpulsionVoteRequest
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string SessionToken { get; set; } = string.Empty;

    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public Guid TargetPlayerId { get; set; }

    [DataMember]
    public long ExpectedVotingRevision { get; set; }
}
