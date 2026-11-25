using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface IPlayerRepository
{
    Player? GetPlayerById(Guid playerId);
    Player? GetPlayerByUsername(string username);
    Player? GetPlayerByEmail(string email);
    bool UsernameExists(string username);
    bool EmailExists(string email);
    bool CreatePlayer(Player player);
    List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults);
    bool UpdatePasswordAndRevokeSessions(Guid playerId, string passwordHash, string passwordSalt);
    PlayerProfileUpdateResult UpdateProfile(Guid playerId, string username, string? countryCode, int? avatarId);
    bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt);
}
