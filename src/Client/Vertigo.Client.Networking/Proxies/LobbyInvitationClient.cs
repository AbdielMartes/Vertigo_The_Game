using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class LobbyInvitationClient : ILobbyInvitationClient
{
    private readonly IServiceCaller _serviceCaller;

    public LobbyInvitationClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetInvitableFriends(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), List<FriendEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.GetInvitableFriends(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }

    public ClientResult<List<Guid>> SendLobbyInvitations(string lobbyCode, List<Guid> targetIds, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, List<Guid>, string), List<Guid>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.SendLobbyInvitations(args.Item1, args.Item2, args.Item3),
            (lobbyCode, targetIds, sessionToken));
    }

    public ClientResult<List<LobbyInvitationEntry>> GetPendingLobbyInvitations(string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, string, List<LobbyInvitationEntry>>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, token) => channel.GetPendingLobbyInvitations(token),
            sessionToken);
    }

    public ClientResult<bool> DeclineLobbyInvitation(string lobbyCode, string sessionToken)
    {
        return _serviceCaller.Call<ILobbyInvitationService, (string, string), bool>(
            ServiceEndpoints.LobbyInvitationServiceAddress,
            (channel, args) => channel.DeclineLobbyInvitation(args.Item1, args.Item2),
            (lobbyCode, sessionToken));
    }
}
