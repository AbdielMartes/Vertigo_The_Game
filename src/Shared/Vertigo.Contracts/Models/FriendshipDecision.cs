using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipDecision
{
    [DataMember]
    public Guid ResponderId { get; set; }

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public bool Accept { get; set; }
}
