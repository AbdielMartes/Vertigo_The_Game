using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAccountService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool RegisterAccount(AccountRegistration registration);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool ChangePassword(PasswordChange change, string sessionToken);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool UpdateProfile(ProfileUpdate profile, string sessionToken);
}
