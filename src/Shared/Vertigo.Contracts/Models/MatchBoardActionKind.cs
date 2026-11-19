using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum MatchBoardActionKind
{
    [EnumMember]
    Placement,

    [EnumMember]
    Dismantle
}
