using CoreWCF;
using Microsoft.Extensions.Logging.Abstractions;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Registration.Services;

namespace Vertigo.Server.Matchmaking.Tests.Registration;

public sealed class AccountServiceTests
{
    private const string ValidPassword = "Clave2026!";
    private const string UpdatedPassword = "NuevaClave2026!";
    private const string Username = "Player";
    private const string EmailSuffix = "@example.com";

    [Fact]
    public void RegisterAccount_ValidInput_SavesNormalizedAccount()
    {
        RecordingPlayerRepository repository = new();
        AccountService service = CreateService(repository);
        string username = new('A', 20);
        string email = new string('a', 70 - EmailSuffix.Length) + EmailSuffix;
        AccountRegistration request = new()
        {
            Username = $" {username} ",
            Email = $" {email} ",
            Password = ValidPassword
        };

        bool registered = service.RegisterAccount(request);

        Assert.True(registered);
        Player saved = Assert.Single(repository.SavedPlayers);
        Assert.Equal(username, saved.Username);
        Assert.Equal(email, saved.Email);
        Assert.Equal(username, repository.CheckedUsername);
        Assert.Equal(email, repository.CheckedEmail);
        Assert.NotNull(saved.Stats);
        Assert.True(new PasswordHasher().VerifyPassword(ValidPassword, saved.PasswordHash, saved.PasswordSalt));
    }

    [Theory]
    [InlineData("Invalid_Name", "player@example.com", ValidPassword)]
    [InlineData("ABCDEFGHIJKLMNOPQRSTU", "player@example.com", ValidPassword)]
    [InlineData("Player", "player@example", ValidPassword)]
    [InlineData("Player", "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyzabcdefg@example.com", ValidPassword)]
    [InlineData("Player", "player@example.com", "short")]
    public void RegisterAccount_InvalidInput_RejectsWithoutWriting(string username, string email, string password)
    {
        RecordingPlayerRepository repository = new();
        AccountService service = CreateService(repository);
        AccountRegistration request = new() { Username = username, Email = email, Password = password };

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => service.RegisterAccount(request));

        Assert.Equal(ServiceErrorCodes.InvalidRegistrationData, fault.Detail.ErrorCode);
        Assert.Empty(repository.SavedPlayers);
    }

    [Fact]
    public void ChangePassword_ValidCredentials_UpdatesPasswordAndRevokesSessions()
    {
        RecordingPlayerRepository players = new();
        RecordingSessionTokenRepository tokens = new();
        AccountService service = CreatePasswordService(players, tokens);
        PasswordChange change = new()
        {
            Username = "player",
            CurrentPassword = ValidPassword,
            NewPassword = UpdatedPassword
        };

        bool changed = service.ChangePassword(change, "session");

        Assert.True(changed);
        Player player = Assert.Single(players.SavedPlayers);
        Assert.Equal(player.PlayerId, Assert.Single(tokens.RevokedPlayers));
        PasswordHasher hasher = new();
        Assert.True(hasher.VerifyPassword(UpdatedPassword, player.PasswordHash, player.PasswordSalt));
    }

    [Theory]
    [InlineData(Username, "WrongClave2026!")]
    [InlineData("OtherPlayer", ValidPassword)]
    public void ChangePassword_InvalidCredentials_RejectsWithoutRevokingSessions(string username, string password)
    {
        RecordingPlayerRepository players = new();
        RecordingSessionTokenRepository tokens = new();
        AccountService service = CreatePasswordService(players, tokens);
        PasswordChange change = new()
        {
            Username = username,
            CurrentPassword = password,
            NewPassword = UpdatedPassword
        };

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => service.ChangePassword(change, "session"));

        Assert.Equal(ServiceErrorCodes.InvalidCredentials, fault.Detail.ErrorCode);
        Assert.Empty(tokens.RevokedPlayers);
        Player player = Assert.Single(players.SavedPlayers);
        PasswordHasher hasher = new();
        Assert.True(hasher.VerifyPassword(ValidPassword, player.PasswordHash, player.PasswordSalt));
    }

    [Fact]
    public void ChangePassword_UpdateRejected_KeepsOriginalPasswordAndSessions()
    {
        RecordingPlayerRepository players = new() { RejectPasswordUpdate = true };
        RecordingSessionTokenRepository tokens = new();
        AccountService service = CreatePasswordService(players, tokens);
        PasswordChange change = new()
        {
            Username = Username,
            CurrentPassword = ValidPassword,
            NewPassword = UpdatedPassword
        };

        bool changed = service.ChangePassword(change, "session");

        Assert.False(changed);
        Assert.Empty(tokens.RevokedPlayers);
        Player player = Assert.Single(players.SavedPlayers);
        Assert.True(new PasswordHasher().VerifyPassword(ValidPassword, player.PasswordHash, player.PasswordSalt));
    }

    private static AccountService CreateService(RecordingPlayerRepository repository)
    {
        return new AccountService(repository, new PasswordHasher(), null!);
    }

    private static AccountService CreatePasswordService(
        RecordingPlayerRepository players,
        RecordingSessionTokenRepository tokens)
    {
        PasswordHasher hasher = new();
        string salt = hasher.GenerateSalt();
        Player player = new()
        {
            PlayerId = Guid.NewGuid(),
            Username = Username,
            PasswordSalt = salt,
            PasswordHash = hasher.HashPassword(ValidPassword, salt)
        };
        players.SavedPlayers.Add(player);
        RecordingSessionResolver sessions = new(player.PlayerId);
        players.SessionTokens = tokens;
        AccountPasswordStore store = new(players, hasher);
        AccountPasswordService passwords = new(store, sessions, NullLogger<AccountService>.Instance);

        return new AccountService(players, hasher, passwords);
    }

    private sealed class RecordingSessionResolver : ISessionIdentityResolver
    {
        private readonly SessionIdentity _identity;

        public RecordingSessionResolver(Guid playerId)
        {
            _identity = new SessionIdentity(playerId, Username, false);
        }

        public SessionIdentity? Resolve(string? token)
        {
            return _identity;
        }

        public SessionIdentity Require(string? token, string operation = "")
        {
            return _identity;
        }

        public SessionIdentity RequireRegistered(string? token, string operation = "")
        {
            return _identity;
        }
    }

    private sealed class RecordingSessionTokenRepository : ISessionTokenRepository
    {
        public List<Guid> RevokedPlayers { get; } = new();

        public bool CreateSessionToken(SessionToken token) => throw new NotSupportedException();

        public SessionToken? GetActiveSessionToken(string tokenHash) => throw new NotSupportedException();

        public bool RevokeSessionToken(string tokenHash) => throw new NotSupportedException();

        public int RevokePlayerSessions(Guid playerId)
        {
            RevokedPlayers.Add(playerId);

            return 1;
        }
    }

    private sealed class RecordingPlayerRepository : IPlayerRepository
    {
        public List<Player> SavedPlayers { get; } = new();
        public bool RejectPasswordUpdate { get; init; }
        public RecordingSessionTokenRepository? SessionTokens { get; set; }
        public string? CheckedUsername { get; private set; }
        public string? CheckedEmail { get; private set; }

        public bool UsernameExists(string username)
        {
            CheckedUsername = username;

            return false;
        }

        public bool EmailExists(string email)
        {
            CheckedEmail = email;

            return false;
        }

        public bool CreatePlayer(Player player)
        {
            SavedPlayers.Add(player);

            return true;
        }

        public Player? GetPlayerById(Guid playerId)
        {
            return SavedPlayers.Find(player => player.PlayerId == playerId);
        }

        public Player? GetPlayerByUsername(string username) => throw new NotSupportedException();

        public Player? GetPlayerByEmail(string email) => throw new NotSupportedException();

        public List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults)
            => throw new NotSupportedException();

        public bool UpdatePasswordAndRevokeSessions(Guid playerId, string passwordHash, string passwordSalt)
        {
            Player? player = RejectPasswordUpdate ? null : GetPlayerById(playerId);
            if (player is not null)
            {
                player.PasswordHash = passwordHash;
                player.PasswordSalt = passwordSalt;
                SessionTokens?.RevokePlayerSessions(playerId);
            }

            return player is not null;
        }

        public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt) => throw new NotSupportedException();
    }
}
