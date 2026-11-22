using Vertigo.Contracts.Internal;
using Vertigo.Server.Matchmaking.Lobbies;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

// The caller must hold the lobby synchronization lock during this operation.
// No asynchronous or network operations are performed by the in-process starter.
public sealed class LobbyMatchActivation
{
    private readonly IMatchSessionStarter _starter;

    public LobbyMatchActivation(IMatchSessionStarter starter)
    {
        _starter = starter;
    }

    public void Activate(LobbyState lobby)
    {
        ArgumentNullException.ThrowIfNull(lobby);
        if (!lobby.CanStartMatch(lobby.HostPlayerId))
        {
            throw new InvalidOperationException("The lobby is not eligible to start a match.");
        }

        Guid[] players = lobby.GetMembersExcept(Guid.Empty).Select(member => member.PlayerId).ToArray();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = lobby.LobbyInstanceId,
            LobbyCode = lobby.LobbyCode,
            HostPlayerId = lobby.HostPlayerId,
            PlayerIds = Array.AsReadOnly(players),
            Lobby = new RecoverableLobby
            {
                Members = lobby.GetMembersExcept(Guid.Empty),
                IsPublic = lobby.IsPublic,
                MaxPlayers = lobby.MaxPlayers,
                CreatedAtUtc = lobby.CreatedAtUtc
            }
        };

        // If Game fails, LobbyState is unchanged and remains Waiting.
        MatchSessionSnapshot match = _starter.Start(request);
        if (match.MatchId == Guid.Empty || match.LobbyInstanceId != lobby.LobbyInstanceId
            || match.LobbyCode != lobby.LobbyCode || match.InitialHostPlayerId != lobby.HostPlayerId
            || !match.PlayerIds.SequenceEqual(players))
        {
            throw new InvalidOperationException("Game returned an inconsistent match session.");
        }

        lobby.StartMatch(match.MatchId);
    }
}
