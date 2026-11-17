#nullable enable

using System;
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Callbacks;

[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
public sealed class LobbyCallbackHandler : ILobbyCallback
{
    public event EventHandler<LobbyMember>? PlayerJoined;

    public event EventHandler<Guid>? PlayerLeft;

    public event EventHandler<LobbyChatMessage>? MessageReceived;

    public event EventHandler<LobbyPreparation>? PreparationChanged;

    void ILobbyCallback.PlayerJoined(LobbyMember player)
    {
        PlayerJoined?.Invoke(this, player);
    }

    void ILobbyCallback.PlayerLeft(Guid playerId)
    {
        PlayerLeft?.Invoke(this, playerId);
    }

    void ILobbyCallback.PreparationChanged(LobbyPreparation preparation)
    {
        PreparationChanged?.Invoke(this, preparation);
    }

    void ILobbyCallback.MessageReceived(LobbyChatMessage message)
    {
        MessageReceived?.Invoke(this, message);
    }
}
