using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class BlockedPlayerSummary
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public DateTime BlockedAt { get; set; }
}
