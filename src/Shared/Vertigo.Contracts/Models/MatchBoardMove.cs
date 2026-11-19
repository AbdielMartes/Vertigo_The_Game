using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchBoardMove
{
    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string PlayerUsername { get; set; } = string.Empty;

    [DataMember]
    public MatchBoardActionKind ActionKind { get; set; }

    [DataMember]
    public int PilePosition { get; set; }

    [DataMember]
    public int? PreviousTop { get; set; }

    [DataMember]
    public List<int> CardNumbers { get; set; } = new();
}
