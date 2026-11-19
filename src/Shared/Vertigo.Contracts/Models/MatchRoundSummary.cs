using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchRoundSummary
{
    [DataMember]
    public int RoundNumber { get; set; }

    [DataMember]
    public Guid? ClosedByPlayerId { get; set; }

    [DataMember]
    public List<MatchRoundPlayerScore> Players { get; set; } = new();
}
