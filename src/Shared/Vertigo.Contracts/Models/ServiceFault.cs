using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models;

[DataContract]
public class ServiceFault
{
    [DataMember]
    public string ErrorCode { get; set; } = string.Empty;

    [DataMember]
    public string MessageKey { get; set; } = string.Empty;
}
