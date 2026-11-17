using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class PlayerSearch
{
    [DataMember]
    public Guid RequesterId { get; set; }

    [DataMember]
    public string SearchTerm { get; set; } = string.Empty;
}
