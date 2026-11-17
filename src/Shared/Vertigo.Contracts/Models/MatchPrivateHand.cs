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
}
