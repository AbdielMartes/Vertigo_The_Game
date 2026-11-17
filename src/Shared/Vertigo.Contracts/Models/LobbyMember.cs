#nullable enable

using System;
using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LobbyMember
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public bool IsGuest { get; set; }
}
