using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class PlayerRepository : IPlayerRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public PlayerRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public Player? GetPlayerById(Guid playerId)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.PlayerId == playerId);

        return player;
    }

    public Player? GetPlayerByUsername(string username)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Username == username);

        return player;
    }

    public Player? GetPlayerByEmail(string email)
    {
        Player? player = _context.Players.AsNoTracking()
            .FirstOrDefault(p => p.Email == email);

        return player;
    }

    public bool UsernameExists(string username)
    {
        bool exists = _context.Players.Any(p => p.Username == username);

        return exists;
    }

    public bool EmailExists(string email)
    {
        bool exists = _context.Players.Any(p => p.Email == email);

        return exists;
    }

    public bool CreatePlayer(Player player)
    {
        _context.Players.Add(player);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt)
    {
        int affectedRows = _context.Players
            .Where(p => p.PlayerId == playerId)
            .ExecuteUpdate(setters => setters.SetProperty(p => p.LastLoginAt, lastLoginAt));

        return affectedRows == ExpectedAffectedRows;
    }
}
