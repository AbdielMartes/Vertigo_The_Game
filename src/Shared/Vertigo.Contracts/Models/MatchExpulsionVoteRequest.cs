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

    [DataMember(EmitDefaultValue = false)]
    public MatchExpulsionDecision Decision { get; set; }

    /// <summary>
    /// Identifies the open ballot for a response or the rejected ballot for a new request.
    /// Use zero for the first request or for legacy support.
    /// </summary>
    [DataMember(EmitDefaultValue = false)]
    public long BallotId { get; set; }
}
