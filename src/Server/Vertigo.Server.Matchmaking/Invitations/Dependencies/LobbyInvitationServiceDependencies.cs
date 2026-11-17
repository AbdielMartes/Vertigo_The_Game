using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.Matchmaking.Invitations.Dependencies;

public sealed record LobbyInvitationServiceDependencies(
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    ILobbyDirectory LobbyDirectory,
    IFriendshipNotifier Notifier);
