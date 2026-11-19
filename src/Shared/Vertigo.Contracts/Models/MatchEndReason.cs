using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum MatchEndReason
{
    [EnumMember]
    None,

    [EnumMember]
    Completed,

    [EnumMember]
    Abandonment
}
