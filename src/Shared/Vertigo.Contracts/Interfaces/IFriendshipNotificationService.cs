using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Interfaces;

[ServiceContract(CallbackContract = typeof(IFriendshipCallback), SessionMode = SessionMode.Required)]
public interface IFriendshipNotificationService
{
    [OperationContract]
    [FaultContract(typeof(ServiceFault))]
    bool Subscribe(string sessionToken);

    [OperationContract(IsOneWay = true)]
    void Unsubscribe();
}
