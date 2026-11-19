using CoreWCF;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Friendships.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class FriendshipService : IFriendshipService
{
    private readonly FriendshipQueries _queries;
    private readonly FriendshipCommands _commands;
    private readonly ISessionIdentityResolver _sessions;

    public FriendshipService(
        FriendshipQueries queries,
        FriendshipCommands commands,
        ISessionIdentityResolver sessions)
    {
        _queries = queries;
        _commands = commands;
        _sessions = sessions;
    }

    public List<FriendEntry> GetFriends(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return _queries.GetFriendsForPlayer(identity.PlayerId);
    }

    public List<PlayerSearchEntry> SearchPlayers(string searchTerm, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerSearch search = new PlayerSearch { RequesterId = identity.PlayerId, SearchTerm = searchTerm };
        return _queries.SearchPlayersForPlayer(search);
    }

    public PlayerRelation SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipInvitation invitation = new FriendshipInvitation
        {
            SenderId = identity.PlayerId,
            TargetId = targetPlayerId
        };

        return _commands.SendFriendRequestForPlayer(invitation);
    }

    public List<FriendshipInvitationEntry> GetPendingFriendRequests(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return _queries.GetPendingRequestsForPlayer(identity.PlayerId);
    }

    public bool RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        FriendshipDecision decision = new FriendshipDecision
        {
            SenderId = senderPlayerId,
            ResponderId = identity.PlayerId,
            Accept = accept
        };

        return _commands.DecideFriendshipForPlayer(decision);
    }

    public bool RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PlayerPair pair = new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId };
        return _commands.RemoveFriendshipForPlayer(pair);
    }
}
