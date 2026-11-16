using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class BlockedPlayerRepository : IBlockedPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public BlockedPlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public List<BlockedPlayer> GetBlockedPlayers(Guid playerId)
    {
        List<BlockedPlayer> blockedPlayers = _context.BlockedPlayers
            .AsNoTracking()
            .Include(b => b.PlayerBlocked)
            .Where(b => b.PlayerBlockerId == playerId)
            .OrderBy(b => b.PlayerBlocked.Username)
            .ToList();

        return blockedPlayers;
    }

    public bool IsPlayerBlocked(Guid playerId, Guid otherPlayerId)
    {
        bool isBlocked = _context.BlockedPlayers
            .Any(b => (b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
                      || (b.PlayerBlockerId == otherPlayerId && b.PlayerBlockedId == playerId));

        return isBlocked;
    }

    public bool BlockPlayer(Guid playerId, Guid otherPlayerId)
    {
        BlockedPlayer blockedPlayer = new BlockedPlayer
        {
            PlayerBlockerId = playerId,
            PlayerBlockedId = otherPlayerId
        };

        _context.BlockedPlayers.Add(blockedPlayer);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool UnblockPlayer(Guid playerId, Guid otherPlayerId)
    {
        int affectedRows = _context.BlockedPlayers
            .Where(b => b.PlayerBlockerId == playerId && b.PlayerBlockedId == otherPlayerId)
            .ExecuteDelete();

        return affectedRows == ExpectedAffectedRows;
    }
}
