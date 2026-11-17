using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum PlayerRelation
{
    [EnumMember]
    None,

    [EnumMember]
    RequestSent,

    [EnumMember]
    Friends,

    [EnumMember]
    Blocked
}
