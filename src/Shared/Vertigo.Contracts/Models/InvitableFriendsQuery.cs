using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class InvitableFriendsQuery
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
