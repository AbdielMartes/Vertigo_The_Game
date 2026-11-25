using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class FriendshipInvitationEntry
{
    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;
    [DataMember(Order = 1)]
    public int? AvatarId { get; set; }
}
