using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class UserSession
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Token { get; set; } = string.Empty;

    [DataMember]
    public DateTime ExpiresAt { get; set; }

    [DataMember]
    public bool IsGuest { get; set; }

    [DataMember(Order = 1)]
    public int? AvatarId { get; set; }
}
