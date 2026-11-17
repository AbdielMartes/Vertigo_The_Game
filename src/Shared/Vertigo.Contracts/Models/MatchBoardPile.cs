using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchBoardPile
{
    [DataMember]
    public int Position { get; set; }

    [DataMember]
    public int? TopCardNumber { get; set; }

    [DataMember]
    public int CardCount { get; set; }
}
