using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public enum MatchBoardActionKind
{
    [EnumMember]
    Placement,

    [EnumMember]
    Dismantle,

    [EnumMember]
    MasterPlan,

    [EnumMember]
    TurnExpired,

    [EnumMember]
    ExpulsionVoteRecorded,

    [EnumMember]
    ExpelledByVote
}
