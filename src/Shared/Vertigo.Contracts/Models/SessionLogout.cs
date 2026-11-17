using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class SessionLogout
{
    [DataMember]
    public string SessionToken { get; set; } = string.Empty;
}
