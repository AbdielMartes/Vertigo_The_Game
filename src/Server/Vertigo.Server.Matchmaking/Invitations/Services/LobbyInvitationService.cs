using CoreWCF;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Invitations.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class LobbyInvitationService : ILobbyInvitationService
{
    private readonly LobbyInvitationOperations _operations;
    private readonly ISessionIdentityResolver _sessions;

    public LobbyInvitationService(LobbyInvitationOperations operations, ISessionIdentityResolver sessions)
    {
        _operations = operations;
        _sessions = sessions;
    }

    public List<FriendEntry> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        InvitableFriendsQuery query = new InvitableFriendsQuery
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return _operations.GetInvitableFriendsForPlayer(query);
    }

    public List<Guid> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitations invitations = new LobbyInvitations
        {
            SenderId = identity.PlayerId,
            LobbyCode = lobbyCode,
            TargetIds = targetIds
        };

        return _operations.SendLobbyInvitationsForPlayer(invitations);
    }

    public List<LobbyInvitationEntry> GetPendingLobbyInvitations(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return _operations.GetPendingInvitationsForPlayer(identity.PlayerId);
    }

    public bool DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        LobbyInvitationDecline decline = new LobbyInvitationDecline
        {
            PlayerId = identity.PlayerId,
            LobbyCode = lobbyCode
        };

        return _operations.DeclineInvitationForPlayer(decline);
    }
}
