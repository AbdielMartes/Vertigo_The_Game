using Vertigo.Contracts.Callbacks;

namespace Vertigo.Server.PlayerManagement.Friendships.Models;

public sealed record FriendshipSubscription(IFriendshipCallback Callback, string SessionId);
