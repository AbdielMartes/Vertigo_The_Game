using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Implementations;

namespace Vertigo.Server.Matchmaking.Tests.Registration;

public sealed class PasswordTransactionTests
{
    [Fact]
    public void UpdatePasswordAndRevokeSessions_IfRevocationFails_RollsBackPassword()
    {
        using SqliteConnection connection = new("Data Source=:memory:");
        connection.Open();
        DbContextOptions<VertigoDatabaseContext> options = new DbContextOptionsBuilder<VertigoDatabaseContext>()
            .UseSqlite(connection)
            .Options;
        using VertigoDatabaseContext database = new(options);
        database.Database.EnsureCreated();
        Player player = SeedPlayer(database);
        database.Database.ExecuteSqlRaw("""
            CREATE TRIGGER reject_session_update BEFORE UPDATE ON SessionTokens
            BEGIN SELECT RAISE(ABORT, 'test revocation failure'); END;
            """);
        PlayerRepository repository = new(database);

        SqliteException failure = Assert.Throws<SqliteException>(
            () => repository.UpdatePasswordAndRevokeSessions(player.PlayerId, "new-hash", "new-salt"));

        Assert.Contains("test revocation failure", failure.Message, StringComparison.Ordinal);
        using VertigoDatabaseContext verification = new(options);
        Player persisted = verification.Players.AsNoTracking().Single();
        Assert.Equal("old-hash", persisted.PasswordHash);
        Assert.Equal("old-salt", persisted.PasswordSalt);
        Assert.False(verification.SessionTokens.AsNoTracking().Single().IsRevoked);

        database.Database.ExecuteSqlRaw("DROP TRIGGER reject_session_update");
        Assert.True(repository.UpdatePasswordAndRevokeSessions(player.PlayerId, "new-hash", "new-salt"));
        Assert.Equal("new-hash", verification.Players.AsNoTracking().Single().PasswordHash);
        Assert.True(verification.SessionTokens.AsNoTracking().Single().IsRevoked);
    }

    private static Player SeedPlayer(VertigoDatabaseContext database)
    {
        DateTime now = DateTime.UtcNow;
        Player player = new()
        {
            PlayerId = Guid.NewGuid(),
            Username = "test-player",
            Email = "test-player@example.test",
            PasswordHash = "old-hash",
            PasswordSalt = "old-salt",
            CreatedAt = now
        };
        database.Players.Add(player);
        database.SessionTokens.Add(new SessionToken
        {
            TokenId = Guid.NewGuid(),
            PlayerId = player.PlayerId,
            TokenValue = "test-token-hash",
            CreatedAt = now,
            ExpiresAt = now.AddHours(1)
        });
        database.SaveChanges();
        return player;
    }
}
