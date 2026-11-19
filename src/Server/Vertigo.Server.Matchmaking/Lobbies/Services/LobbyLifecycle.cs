using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

public sealed class LobbyLifecycle
{
    private readonly TimeProvider _timeProvider;
    private readonly IFriendshipNotifier _notifier;

    public LobbyLifecycle(TimeProvider timeProvider, IFriendshipNotifier notifier)
    {
        _timeProvider = timeProvider;
        _notifier = notifier;
    }

    internal LobbyCreation PrepareCreation(bool isPublic, int maxPlayers)
    {
        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();

        return new LobbyCreation(isPublic, maxPlayers, createdAtUtc);
    }

    internal LobbyState CreateLobby(string lobbyCode, LobbyConnection host, LobbyCreation creation)
    {
        LobbySettings settings = new(
            lobbyCode, host.Member.PlayerId, creation.IsPublic, creation.MaxPlayers, creation.CreatedAtUtc);
        LobbyState lobby = new(settings);
        lobby.AddOrReplace(host);

        return lobby;
    }

    internal void WithdrawInvitations(string lobbyCode, IReadOnlyList<Guid> invitedIds)
    {
        foreach (Guid invitedId in invitedIds)
        {
            _notifier.NotifyLobbyInvitationWithdrawn(invitedId, lobbyCode);
        }
    }
}
