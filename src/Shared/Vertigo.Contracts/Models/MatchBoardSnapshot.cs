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
    public List<MatchBoardPlayer> Players { get; set; } = new();

    [DataMember]
    public List<MatchBoardPile> Piles { get; set; } = new();

    [DataMember]
    public List<Guid> WinnerIds { get; set; } = new();

    [DataMember]
    public List<MatchRoundSummary> RoundSummaries { get; set; } = new();

    [OnDeserializing]
    private void InitializeRoundSummaries(StreamingContext context)
    {
        RoundSummaries = new List<MatchRoundSummary>();
    }
}
