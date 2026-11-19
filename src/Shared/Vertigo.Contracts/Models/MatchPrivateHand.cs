using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchPrivateHand
{
    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public List<int> Cards { get; set; } = new();

    [DataMember]
    public int MasterPlans { get; set; }

    [DataMember]
    public long? Revision { get; set; }

    [DataMember]
    public long VotingRevision { get; set; }

    [DataMember]
    public List<Guid> VotedExpulsionTargetIds { get; set; } = new();
}
