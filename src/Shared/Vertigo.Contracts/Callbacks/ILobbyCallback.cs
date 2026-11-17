#nullable enable

using System.ServiceModel;
using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Callbacks;

[ServiceContract]
public interface ILobbyCallback
{
    [OperationContract(IsOneWay = true)]
    void PlayerJoined(LobbyMember player);

    [OperationContract(IsOneWay = true)]
    void PlayerLeft(Guid playerId);

    [OperationContract(IsOneWay = true)]
    void MessageReceived(LobbyChatMessage message);

    [OperationContract(IsOneWay = true)]
    void PreparationChanged(LobbyPreparation preparation);
}
