using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class PlayerRepository : IPlayerRepository
{
    private const int ExpectedAffectedRows = 1;
    private const int DuplicateIndexError = 2601;
    private const int DuplicateConstraintError = 2627;

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

        return affectedRows >= ExpectedAffectedRows;
    }

    public PlayerProfileUpdateResult UpdateProfile(Guid playerId, string username, string? countryCode, int? avatarId)
    {
        PlayerProfileUpdateResult result = PlayerProfileUpdateResult.Updated;
        try
        {
            int affectedRows = _context.Players
                .Where(player => player.PlayerId == playerId && !_context.Players.Any(other =>
                    other.PlayerId != playerId && other.Username == username))
                .ExecuteUpdate(setters => setters
                    .SetProperty(player => player.Username, username)
                    .SetProperty(player => player.CountryCode, countryCode)
                    .SetProperty(player => player.AvatarId, avatarId));

            if (affectedRows != ExpectedAffectedRows)
            {
                bool playerExists = _context.Players.Any(player => player.PlayerId == playerId);
                if (playerExists)
                {
                    result = PlayerProfileUpdateResult.UsernameTaken;
                }
                else
                {
                    result = PlayerProfileUpdateResult.PlayerNotFound;
                }
            }
        }
        catch (SqlException ex) when (ex.Number is DuplicateIndexError or DuplicateConstraintError)
        {
            result = PlayerProfileUpdateResult.UsernameTaken;
        }

        return result;
    }

    public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt)
    {
        int affectedRows = _context.Players
            .Where(p => p.PlayerId == playerId)
            .ExecuteUpdate(setters => setters.SetProperty(p => p.LastLoginAt, lastLoginAt));

        return affectedRows == ExpectedAffectedRows;
    }

    public List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults)
    {
        return _context.Players.AsNoTracking()
            .Where(player => player.PlayerId != excludedPlayerId && player.Username.Contains(searchTerm))
            .OrderBy(player => player.Username)
            .Take(maximumResults)
            .ToList();
    }

    public bool UpdatePasswordAndRevokeSessions(Guid playerId, string passwordHash, string passwordSalt)
    {
        using IDbContextTransaction transaction = _context.Database.BeginTransaction();
        int affectedRows = _context.Players
            .Where(player => player.PlayerId == playerId)
            .ExecuteUpdate(setters => setters
                .SetProperty(player => player.PasswordHash, passwordHash)
                .SetProperty(player => player.PasswordSalt, passwordSalt));

        bool isChanged = affectedRows == ExpectedAffectedRows;
        if (isChanged)
        {
            _context.SessionTokens
                .Where(session => session.PlayerId == playerId && !session.IsRevoked)
                .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));
            transaction.Commit();
        }

        return isChanged;
    }
}
