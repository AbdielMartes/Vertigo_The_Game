using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class FriendshipClient : IFriendshipClient
{
    private readonly IServiceCaller _serviceCaller;

    public FriendshipClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<FriendEntry>> GetFriends(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetFriends(token),
            sessionToken);
    }

    public ClientResult<List<PlayerSearchEntry>> SearchPlayers(string searchTerm, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (string, string), List<PlayerSearchEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SearchPlayers(args.Item1, args.Item2),
            (searchTerm, sessionToken));
    }

    public ClientResult<PlayerRelation> SendFriendRequest(Guid targetPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), PlayerRelation>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.SendFriendRequest(args.Item1, args.Item2),
            (targetPlayerId, sessionToken));
    }

    public ClientResult<List<FriendshipInvitationEntry>> GetPendingFriendRequests(string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, string, List<FriendshipInvitationEntry>>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, token) => channel.GetPendingFriendRequests(token),
            sessionToken);
    }

    public ClientResult<bool> RespondToFriendship(Guid senderPlayerId, bool accept, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, bool, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RespondToFriendship(args.Item1, args.Item2, args.Item3),
            (senderPlayerId, accept, sessionToken));
    }

    public ClientResult<bool> RemoveFriendship(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IFriendshipService, (Guid, string), bool>(
            ServiceEndpoints.FriendshipServiceAddress,
            (channel, args) => channel.RemoveFriendship(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
