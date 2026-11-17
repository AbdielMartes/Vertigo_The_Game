using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public sealed class LobbyChatMessage
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid SenderId { get; set; }

    [DataMember]
    public string SenderUsername { get; set; } = string.Empty;

    [DataMember]
    public string Text { get; set; } = string.Empty;

    [DataMember]
    public DateTime SentAt { get; set; }
}
