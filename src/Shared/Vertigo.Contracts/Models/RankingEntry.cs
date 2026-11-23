using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class RankingEntry
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public int MatchesWon { get; set; }

    [DataMember]
    public int FloorsBuilt { get; set; }
}
