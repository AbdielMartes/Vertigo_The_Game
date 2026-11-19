using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface IFriendshipCallback
{
    [OperationContract(IsOneWay = true)]
    void Notify(FriendshipNotification notification);
}
