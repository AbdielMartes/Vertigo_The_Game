using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

// One dependency for Matchmaking's internal Game interactions.
// All methods are invoked while the owning LobbyService holds its lobby lock.
public sealed class LobbyGameOperations
{
    private readonly LobbyMatchActivation _activation;
    private readonly IMatchStateReader _state;
    private readonly IMatchCommandService _commands;

    public LobbyGameOperations(
        LobbyMatchActivation activation,
        IMatchStateReader state,
        IMatchCommandService commands)
    {
        _activation = activation;
        _state = state;
        _commands = commands;
    }

    public void Activate(LobbyState lobby)
    {
        _activation.Activate(lobby);
    }

    public MatchBoardSnapshot? GetBoard(LobbyState lobby)
    {
        MatchBoardSnapshot? board = _state.GetBoard(lobby.MatchId);
        if (board is not null)
        {
            IReadOnlyDictionary<Guid, LobbyMember> members = lobby.MatchPlayers;
            foreach (MatchBoardMove move in board.RecentMoves)
            {
                if (members.TryGetValue(move.PlayerId, out LobbyMember? member))
                {
                    move.PlayerUsername = member.Username;
                }
            }

            foreach (MatchRoundPlayerScore score in board.RoundSummaries.SelectMany(round => round.Players))
            {
                if (members.TryGetValue(score.PlayerId, out LobbyMember? member))
                {
                    score.PlayerUsername = member.Username;
                }
            }

            foreach (MatchBoardPlayer player in board.Players)
            {
                if (members.TryGetValue(player.PlayerId, out LobbyMember? member))
                {
                    player.Username = member.Username;
                    player.AvatarId = member.AvatarId;
                }
            }
        }

        return board;
    }

    public MatchPrivateHand? GetPrivateHand(Guid matchId, Guid playerId)
    {
        return _state.GetPrivateHand(matchId, playerId);
    }

    public MatchPlacementResult TryPlace(Guid matchId, Guid playerId, MatchPlacement move)
    {
        return _commands.TryPlace(matchId, playerId, move);
    }

    public MatchPlacementResult TryUseMasterPlan(Guid matchId, Guid playerId, MatchMasterPlan move)
    {
        return _commands.TryUseMasterPlan(matchId, playerId, move);
    }

    public MatchDismantleResult TryDismantle(Guid matchId, Guid playerId, MatchDismantle move)
    {
        return _commands.TryDismantle(matchId, playerId, move);
    }

    public MatchExpulsionVoteResult TryVoteToExpel(Guid matchId, Guid voterId, MatchExpulsionVote vote)
    {
        return _commands.TryVoteToExpel(matchId, voterId, vote);
    }

    public bool TryConfirmPresence(Guid matchId, Guid playerId)
    {
        return _commands.TryConfirmPresence(matchId, playerId);
    }

    public Guid? TryExpireTurn(Guid matchId)
    {
        return _commands.TryExpireTurn(matchId);
    }

    public MatchDepartureResult TryLeaveMatch(Guid matchId, Guid playerId)
    {
        return _commands.TryLeaveMatch(matchId, playerId);
    }

    public bool TryReconnect(LobbyState lobby, Guid playerId)
    {
        MatchReconnectResult result = _commands.TryReconnect(lobby.MatchId, playerId);
        bool accepted = result is MatchReconnectResult.Reconnected
            or MatchReconnectResult.AlreadyConnected;
        if (!accepted)
        {
            MatchBoardSnapshot? board = _state.GetBoard(lobby.MatchId);
            accepted = board is not null && (board.TurnDeadlineUtc is null
                || board.ExpelledPlayerIds.Contains(playerId));
        }

        return accepted;
    }

    public void Disconnect(LobbyState lobby, Guid playerId)
    {
        _commands.TryDisconnect(lobby.MatchId, playerId);
    }
}
