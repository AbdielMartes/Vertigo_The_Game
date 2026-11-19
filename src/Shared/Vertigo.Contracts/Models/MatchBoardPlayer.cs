using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchBoardPlayer
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public int FloorsBuilt { get; set; }

    [DataMember]
    public int GlobalFloors { get; set; }
}
