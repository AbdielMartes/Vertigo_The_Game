using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class PlayerMatchRecord
{
    [DataMember]
    public int MatchesPlayed { get; set; }

    [DataMember]
    public int MatchesWon { get; set; }
}
