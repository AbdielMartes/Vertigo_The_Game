using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class PlayerMatchRecord
{
    [DataMember]
    public int MatchesPlayed { get; set; }

    [DataMember]
    public int MatchesWon { get; set; }

    [DataMember(Order = 1)]
    public int? CreatedYear { get; set; }

    [DataMember(Order = 2)]
    public int? AvatarId { get; set; }

    [DataMember(Order = 3)]
    public string? CountryCode { get; set; }
}
