using System.Runtime.Serialization;

namespace Vertigo.Contracts.Models
{
    [DataContract]
    public class PasswordChange
    {
        [DataMember] 
        public string Username { get; set; } = string.Empty;
        
        [DataMember] 
        public string CurrentPassword { get; set; } = string.Empty;
        
        [DataMember] 
        public string NewPassword { get; set; } = string.Empty;
    }
}
