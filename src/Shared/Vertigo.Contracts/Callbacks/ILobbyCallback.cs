using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface ILobbyCallback
{
    [OperationContract(IsOneWay = true)]
    void Notify(LobbyNotification notification);
}
