#nullable enable

using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbySummary
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public string HostUsername { get; set; } = string.Empty;

    [DataMember]
    public int CurrentPlayers { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }
}
