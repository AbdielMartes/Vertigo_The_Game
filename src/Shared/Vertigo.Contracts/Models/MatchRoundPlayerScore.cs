using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchRoundPlayerScore
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public int FloorsBuilt { get; set; }

    [DataMember]
    public int BonusFloors { get; set; }

    [DataMember]
    public int TotalFloors { get; set; }
}
