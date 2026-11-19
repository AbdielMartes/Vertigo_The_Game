using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchBoardSnapshot
{
    [DataMember]
    public Guid MatchId { get; set; }

    [DataMember]
    public long Revision { get; set; }

    [DataMember]
    public int? RoundNumber { get; set; }

    [DataMember]
    public Guid CurrentTurnPlayerId { get; set; }

    [DataMember]
    public DateTimeOffset? TurnDeadlineUtc { get; set; }

    [DataMember]
    public MatchEndReason EndReason { get; set; }

    [DataMember]
    public List<Guid> ExpelledPlayerIds { get; set; } = new();

    [DataMember]
    public long VotingRevision { get; set; }

    [DataMember]
    public int ExpulsionVotesRequired { get; set; }

    [DataMember]
    public List<MatchExpulsionVoteSummary> ExpulsionVotes { get; set; } = new();

    [DataMember]
    public List<MatchBoardPlayer> Players { get; set; } = new();

    [DataMember]
    public List<MatchBoardPile> Piles { get; set; } = new();

    [DataMember]
    public List<Guid> WinnerIds { get; set; } = new();

    [DataMember]
    public List<MatchRoundSummary> RoundSummaries { get; set; } = new();

    [DataMember]
    public List<MatchBoardMove> RecentMoves { get; set; } = new();

    [OnDeserializing]
    private void InitializeCollections(StreamingContext context)
    {
        RoundSummaries = new List<MatchRoundSummary>();
        RecentMoves = new List<MatchBoardMove>();
        ExpelledPlayerIds = new List<Guid>();
        ExpulsionVotes = new List<MatchExpulsionVoteSummary>();
    }
}
