using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class BlockedPlayerClient : IBlockedPlayerClient
{
    private readonly IServiceCaller _serviceCaller;

    public BlockedPlayerClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, string, List<BlockedPlayerSummary>>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, token) => channel.GetBlockedPlayers(token),
            sessionToken);
    }

    public ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.BlockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }

    public ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        return _serviceCaller.Call<IBlockedPlayerService, (Guid, string), bool>(
            ServiceEndpoints.BlockedPlayerServiceAddress,
            (channel, args) => channel.UnblockPlayer(args.Item1, args.Item2),
            (otherPlayerId, sessionToken));
    }
}
