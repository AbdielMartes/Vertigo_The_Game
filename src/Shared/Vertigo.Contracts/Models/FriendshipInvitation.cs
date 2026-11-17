using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitation
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public Guid TargetId { get; set; }
}
