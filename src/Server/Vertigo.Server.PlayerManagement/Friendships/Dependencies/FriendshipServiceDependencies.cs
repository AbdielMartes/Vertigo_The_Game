using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Friendships.Dependencies;

public sealed record FriendshipServiceDependencies(
    IPlayerRepository PlayerRepository,
    IFriendshipRepository FriendshipRepository,
    IBlockedPlayerRepository BlockedPlayerRepository,
    IFriendshipNotifier Notifier);
