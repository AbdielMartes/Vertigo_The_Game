using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract]
public interface IAuthService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession Login(LoginCredentials credentials);

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    UserSession ResumeSession(string token);

    [OperationContract]
    UserSession CreateGuestSession();

    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Logout(SessionLogout logout);
}
