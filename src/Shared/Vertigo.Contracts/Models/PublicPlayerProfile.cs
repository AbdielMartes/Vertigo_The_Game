using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class PublicPlayerProfile
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public PlayerMatchRecord Record { get; set; } = new();
}
