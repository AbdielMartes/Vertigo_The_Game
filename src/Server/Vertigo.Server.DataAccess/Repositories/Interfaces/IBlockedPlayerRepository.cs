using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IBlockedPlayerRepository
{
    List<BlockedPlayer> GetBlocksBetween(Guid playerId, List<Guid> otherPlayerIds);
    List<BlockedPlayer> GetBlockedPlayers(Guid playerId);
    bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId);
    bool BlockPlayer(Guid playerId, Guid otherPlayerId);
    bool UnblockPlayer(Guid playerId, Guid otherPlayerId);
}
