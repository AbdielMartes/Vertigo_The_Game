using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyInvitationDecline
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;
}
