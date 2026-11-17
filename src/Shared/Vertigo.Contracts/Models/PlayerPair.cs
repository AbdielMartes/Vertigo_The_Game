using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerPair
{
    [DataMember]
    public Guid PlayerId { get; set; }

    [DataMember]
    public Guid OtherPlayerId { get; set; }
}
