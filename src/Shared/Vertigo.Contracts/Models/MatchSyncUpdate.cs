using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchSyncUpdate
{
    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? BaseRevision { get; set; }

    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public int? RoundNumber { get; set; }

    [DataMember]
    public Guid CurrentTurnPlayerId { get; set; }

    [DataMember]
    public DateTimeOffset? TurnDeadlineUtc { get; set; }

    [DataMember]
    public DateTimeOffset? ServerTimeUtc { get; set; }

    [DataMember]
    public MatchEndReason EndReason { get; set; }

    [DataMember]
    public long VotingRevision { get; set; }

    [DataMember]
    public int ExpulsionVotesRequired { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<MatchBoardPile>? Piles { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<MatchBoardPlayer>? Players { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<MatchBoardMove>? RecentMoves { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<MatchRoundSummary>? Rounds { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MatchSyncStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool HandChanged { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MatchPrivateHand? Hand { get; set; }
}
