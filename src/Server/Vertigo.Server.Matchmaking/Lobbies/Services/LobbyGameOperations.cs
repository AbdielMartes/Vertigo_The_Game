using Vertigo.Contracts.Internal;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

// One dependency for Matchmaking's internal Game interactions.
// All methods are invoked while the owning LobbyService holds its lobby lock.
public sealed class LobbyGameOperations
{
    private readonly LobbyMatchActivation _activation;
    private readonly IMatchPresence _presence;

    public LobbyGameOperations(LobbyMatchActivation activation, IMatchPresence presence)
    {
        _activation = activation;
        _presence = presence;
    }

    public void Activate(LobbyState lobby)
    {
        _activation.Activate(lobby);
    }

    public bool TryReconnect(LobbyState lobby, Guid playerId)
    {
        MatchReconnectResult result = _presence.TryReconnect(lobby.MatchId, playerId);
        bool accepted = result is MatchReconnectResult.Reconnected
            or MatchReconnectResult.AlreadyConnected;

        return accepted;
    }

    public void Disconnect(LobbyState lobby, Guid playerId)
    {
        _presence.TryDisconnect(lobby.MatchId, playerId);
    }
}
