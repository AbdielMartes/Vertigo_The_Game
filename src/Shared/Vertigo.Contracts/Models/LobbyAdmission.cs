#nullable enable

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyAdmission
{
    [DataMember]
    public string LobbyCode { get; set; } = string.Empty;

    [DataMember]
    public Guid HostPlayerId { get; set; }

    [DataMember]
    public bool IsPublic { get; set; }

    [DataMember]
    public int MaxPlayers { get; set; }

    [DataMember]
    public List<LobbyMember> ExistingMembers { get; set; } = new();
}
