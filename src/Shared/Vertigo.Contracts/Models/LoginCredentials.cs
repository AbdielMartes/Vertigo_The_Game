using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class LoginCredentials
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
