using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public sealed class AccountPasswordStore
{
    private readonly IPlayerRepository _players;
    private readonly IPasswordHasher _passwordHasher;

    public AccountPasswordStore(
        IPlayerRepository players,
        IPasswordHasher passwordHasher)
    {
        _players = players;
        _passwordHasher = passwordHasher;
    }

    internal PasswordChangeOutcome TryChange(Guid playerId, PasswordChange change)
    {
        Player? player = _players.GetPlayerById(playerId);
        PasswordChangeOutcome outcome = PasswordChangeOutcome.InvalidCredentials;
        if (player is not null
            && string.Equals(player.Username, change.Username, StringComparison.OrdinalIgnoreCase)
            && _passwordHasher.VerifyPassword(change.CurrentPassword, player.PasswordHash, player.PasswordSalt))
        {
            string salt = _passwordHasher.GenerateSalt();
            string hash = _passwordHasher.HashPassword(change.NewPassword, salt);
            bool isChanged = _players.UpdatePasswordAndRevokeSessions(player.PlayerId, hash, salt);
            outcome = isChanged ? PasswordChangeOutcome.Updated : PasswordChangeOutcome.NotUpdated;
        }

        return outcome;
    }
}
