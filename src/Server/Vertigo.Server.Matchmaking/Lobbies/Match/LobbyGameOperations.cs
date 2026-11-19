using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

// One dependency for Matchmaking's internal Game interactions.
// All methods are invoked while the owning LobbyService holds its lobby lock.
public sealed class LobbyGameOperations
{
    private readonly LobbyMatchActivation _activation;
    private readonly IMatchPresence _presence;
    private readonly MatchReadServices _readers;

    public LobbyGameOperations(LobbyMatchActivation activation, IMatchPresence presence,
        MatchReadServices readers)
    {
        _activation = activation;
        _presence = presence;
        _readers = readers;
    }

    public void Activate(LobbyState lobby)
    {
        _activation.Activate(lobby);
    }

    public MatchBoardSnapshot? GetBoard(LobbyState lobby)
    {
        MatchBoardSnapshot? board = _readers.Board.GetBoard(lobby.MatchId);
        if (board is not null)
        {
            Dictionary<Guid, string> names = lobby.GetMembersExcept(Guid.Empty)
                .ToDictionary(member => member.PlayerId, member => member.Username);
            foreach (MatchBoardPlayer player in board.Players)
            {
                if (names.TryGetValue(player.PlayerId, out string? username))
                {
                    player.Username = username;
                }
            }
        }

        return board;
    }

    public MatchPrivateHand? GetPrivateHand(Guid matchId, Guid playerId)
    {
        return _readers.Hands.GetPrivateHand(matchId, playerId);
    }

    public MatchPlacementResult TryPlace(Guid matchId, Guid playerId, MatchPlacement move)
    {
        return _readers.Placements.TryPlace(matchId, playerId, move);
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
