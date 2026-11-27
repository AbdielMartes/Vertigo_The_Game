using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum MatchExpulsionDecision
{
    [EnumMember]
    Support,
    [EnumMember]
    Request,
    [EnumMember]
    Accept,
    [EnumMember]
    Reject
}
