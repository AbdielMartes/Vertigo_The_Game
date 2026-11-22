using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchSyncCursor
{
    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public string Piles { get; set; } = string.Empty;

    [DataMember]
    public string Players { get; set; } = string.Empty;

    [DataMember]
    public string RecentMoves { get; set; } = string.Empty;

    [DataMember]
    public string Rounds { get; set; } = string.Empty;

    [DataMember]
    public string Status { get; set; } = string.Empty;

    [DataMember]
    public string Hand { get; set; } = string.Empty;
}
