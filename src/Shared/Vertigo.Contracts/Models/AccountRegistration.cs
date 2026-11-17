using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class AccountRegistration
{
    [DataMember]
    public string Username { get; set; } = string.Empty;

    [DataMember]
    public string Email { get; set; } = string.Empty;

    [DataMember]
    public string Password { get; set; } = string.Empty;
}
