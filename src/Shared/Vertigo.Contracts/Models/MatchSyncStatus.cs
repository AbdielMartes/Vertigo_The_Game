using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class MatchSyncStatus
{
    [DataMember]
    public List<Guid> Expelled { get; set; } = new();

    [DataMember]
    public List<Guid> Inactive { get; set; } = new();

    [DataMember(EmitDefaultValue = false)]
    public List<Guid>? ExpelledByVote { get; set; }

    [DataMember]
    public List<Guid> Winners { get; set; } = new();

    [DataMember]
    public Dictionary<Guid, DateTimeOffset> PresenceDeadlinesUtc { get; set; } = new();

    [DataMember]
    public List<MatchExpulsionVoteSummary> ExpulsionVotes { get; set; } = new();
}
