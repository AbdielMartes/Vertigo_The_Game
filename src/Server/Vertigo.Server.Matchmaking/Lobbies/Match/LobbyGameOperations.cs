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
            IReadOnlyDictionary<Guid, string> names = lobby.MatchPlayerNames;
            foreach (MatchBoardMove move in board.RecentMoves)
            {
                if (names.TryGetValue(move.PlayerId, out string? username))
                {
                    move.PlayerUsername = username;
                }
            }

            foreach (MatchRoundPlayerScore score in board.RoundSummaries.SelectMany(round => round.Players))
            {
                if (names.TryGetValue(score.PlayerId, out string? username))
                {
                    score.PlayerUsername = username;
                }
            }

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

    public MatchPlacementResult TryUseMasterPlan(Guid matchId, Guid playerId, MatchMasterPlan move)
    {
        return _readers.Placements.TryUseMasterPlan(matchId, playerId, move);
    }

    public MatchDismantleResult TryDismantle(Guid matchId, Guid playerId, MatchDismantle move)
    {
        return _readers.Dismantles.TryDismantle(matchId, playerId, move);
    }

    public MatchExpulsionVoteResult TryVoteToExpel(Guid matchId, Guid voterId, MatchExpulsionVote vote)
    {
        return _readers.ExpulsionVotes.TryVoteToExpel(matchId, voterId, vote);
    }

    public Guid? TryExpireTurn(Guid matchId)
    {
        return _readers.TurnClock.TryExpireTurn(matchId);
    }

    public MatchDepartureResult TryLeaveMatch(Guid matchId, Guid playerId)
    {
        return _readers.Departures.TryLeaveMatch(matchId, playerId);
    }

    public bool TryReconnect(LobbyState lobby, Guid playerId)
    {
        MatchReconnectResult result = _presence.TryReconnect(lobby.MatchId, playerId);
        bool accepted = result is MatchReconnectResult.Reconnected
            or MatchReconnectResult.AlreadyConnected;
        if (!accepted)
        {
            MatchBoardSnapshot? board = _readers.Board.GetBoard(lobby.MatchId);
            accepted = board is not null && (board.TurnDeadlineUtc is null
                || board.ExpelledPlayerIds.Contains(playerId));
        }

        return accepted;
    }

    public void Disconnect(LobbyState lobby, Guid playerId)
    {
        _presence.TryDisconnect(lobby.MatchId, playerId);
    }
}
