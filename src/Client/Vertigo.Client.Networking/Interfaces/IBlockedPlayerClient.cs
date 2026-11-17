using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IBlockedPlayerClient
{
    ClientResult<List<BlockedPlayerSummary>> GetBlockedPlayers(string sessionToken);

    ClientResult<bool> BlockPlayer(Guid otherPlayerId, string sessionToken);

    ClientResult<bool> UnblockPlayer(Guid otherPlayerId, string sessionToken);
}
