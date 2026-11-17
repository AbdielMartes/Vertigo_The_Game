using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearchEntry
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public PlayerRelation Relation { get; set; }
}
