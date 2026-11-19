using CoreWCF;
using Microsoft.Extensions.Logging.Abstractions;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;
using Vertigo.Server.PlayerManagement.Registration.Services;

namespace Vertigo.Server.Matchmaking.Tests.Registration;

public sealed class AccountServiceTests
{
    private const string ValidPassword = "Clave2026!";
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

    private static AccountService CreateService(RecordingPlayerRepository repository)
    {
        // Registration does not use the session dependencies.
        AccountServiceDependencies dependencies = new(repository, null!, new PasswordHasher(), null!);

        return new AccountService(dependencies, NullLogger<AccountService>.Instance);
    }

    private sealed class RecordingPlayerRepository : IPlayerRepository
    {
        public List<Player> SavedPlayers { get; } = new();
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

        public Player? GetPlayerById(Guid playerId) => throw new NotSupportedException();

        public Player? GetPlayerByUsername(string username) => throw new NotSupportedException();

        public Player? GetPlayerByEmail(string email) => throw new NotSupportedException();

        public List<Player> SearchPlayersByUsername(string searchTerm, Guid excludedPlayerId, int maximumResults)
            => throw new NotSupportedException();

        public bool UpdatePassword(Guid playerId, string passwordHash, string passwordSalt)
            => throw new NotSupportedException();

        public bool UpdateLastLogin(Guid playerId, DateTime lastLoginAt) => throw new NotSupportedException();
    }
}
