using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class ProfileUpdate
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string? CountryCode { get; set; }

    [DataMember]
    public int? AvatarId { get; set; }
}
