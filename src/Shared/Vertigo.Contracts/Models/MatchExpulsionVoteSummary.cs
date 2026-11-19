using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchExpulsionVoteSummary
{
    [DataMember]
    public Guid TargetPlayerId { get; set; }

    [DataMember]
    public int VoteCount { get; set; }
}
