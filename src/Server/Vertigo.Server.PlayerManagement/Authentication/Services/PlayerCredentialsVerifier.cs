using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class PlayerCredentialsVerifier
{
    private readonly IPlayerRepository _players;
    private readonly IPasswordHasher _passwordHasher;

    public PlayerCredentialsVerifier(IPlayerRepository players, IPasswordHasher passwordHasher)
    {
        _players = players;
        _passwordHasher = passwordHasher;
    }

    internal Player? FindVerifiedPlayer(string username, string password)
    {
        Player? player = _players.GetPlayerByUsername(username);
        if (player is not null && !_passwordHasher.VerifyPassword(password, player.PasswordHash, player.PasswordSalt))
        {
            player = null;
        }

        return player;
    }
}
