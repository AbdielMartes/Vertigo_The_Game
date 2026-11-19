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

    public event EventHandler<MatchBoardChanged>? MatchBoardUpdated;

    void ILobbyCallback.Notify(LobbyNotification notification)
    {
        switch (notification)
        {
            case { Kind: LobbyNotificationKind.PlayerJoined, JoinedPlayer: { } player,
                LeftPlayerId: null, ChatMessage: null, Preparation: null, BoardChange: null }:
                PlayerJoined?.Invoke(this, player);
                break;

            case { Kind: LobbyNotificationKind.PlayerLeft, LeftPlayerId: Guid playerId,
                JoinedPlayer: null, ChatMessage: null, Preparation: null, BoardChange: null }
                when playerId != Guid.Empty:
                PlayerLeft?.Invoke(this, playerId);
                break;

            case { Kind: LobbyNotificationKind.MessageReceived, ChatMessage: { } message,
                JoinedPlayer: null, LeftPlayerId: null, Preparation: null, BoardChange: null }:
                MessageReceived?.Invoke(this, message);
                break;

            case { Kind: LobbyNotificationKind.PreparationChanged, Preparation: { } preparation,
                JoinedPlayer: null, LeftPlayerId: null, ChatMessage: null, BoardChange: null }:
                PreparationChanged?.Invoke(this, preparation);
                break;

            case { Kind: LobbyNotificationKind.MatchBoardUpdated, BoardChange: { } change,
                JoinedPlayer: null, LeftPlayerId: null, ChatMessage: null, Preparation: null }:
                MatchBoardUpdated?.Invoke(this, change);
                break;
        }
    }
}
