using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Vertigo.Registration;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Game;
using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Contexts;

public class VertigoDatabaseContext : DbContext
{
    private const int CountryCodeLength = 2;

    public DbSet<Player> Players => Set<Player>();
    public DbSet<PlayerStats> PlayerStats => Set<PlayerStats>();
    public DbSet<SessionToken> SessionTokens => Set<SessionToken>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<BlockedPlayer> BlockedPlayers => Set<BlockedPlayer>();
    public DbSet<MatchHistory> MatchHistory => Set<MatchHistory>();

    public VertigoDatabaseContext(DbContextOptions<VertigoDatabaseContext> options) : base(options)
    {
    }

    public void UpgradeExistingSchema()
    {
        if (!Database.IsSqlServer())
        {
            throw new InvalidOperationException("Schema provisioning requires SQL Server.");
        }

        Database.OpenConnection();
        try
        {
            ApplyExistingSchemaChanges();
        }
        finally
        {
            Database.CloseConnection();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigurePlayers(modelBuilder);
        ConfigurePlayerStats(modelBuilder);
        ConfigureSessionTokens(modelBuilder);
        ConfigureFriendships(modelBuilder);
        ConfigureBlockedPlayers(modelBuilder);
        ConfigureMatchHistory(modelBuilder);
        if (Database.IsSqlServer())
        {
            modelBuilder.Entity<RegistrationColumn>(column =>
            {
                column.HasNoKey();
                column.ToView("COLUMNS", "INFORMATION_SCHEMA");
                column.Property(value => value.Schema).HasColumnName("TABLE_SCHEMA");
                column.Property(value => value.Table).HasColumnName("TABLE_NAME");
                column.Property(value => value.Name).HasColumnName("COLUMN_NAME");
                column.Property(value => value.Collation).HasColumnName("COLLATION_NAME");
            });
        }
    }

    private static void ConfigurePlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(player =>
        {
            player.ToTable("Players");
            player.HasKey(p => p.PlayerId);
            player.Property(p => p.PlayerId).HasDefaultValueSql("NEWID()");
            player.Property(p => p.Username).HasMaxLength(RegistrationRules.UsernameMaximumLength).IsRequired();
            player.Property(p => p.Email).HasMaxLength(RegistrationRules.EmailMaximumLength).IsRequired();
            player.Property(p => p.PasswordHash).HasMaxLength(256).IsRequired();
            player.Property(p => p.PasswordSalt).HasMaxLength(128).IsRequired();
            player.Property(p => p.CountryCode).IsUnicode(false).HasMaxLength(CountryCodeLength);
            player.Property(p => p.IsEmailVerified).HasDefaultValue(false);
            player.Property(p => p.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            player.Property(p => p.LastLoginAt).HasColumnType("datetime2");
            player.HasAlternateKey(p => p.Username);
            player.HasAlternateKey(p => p.Email);
            player.HasIndex(p => p.Username).HasDatabaseName("IX_Players_Username");
        });
    }

    private static void ConfigurePlayerStats(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PlayerStats>(stats =>
        {
            stats.ToTable("PlayerStats");
            stats.HasKey(s => s.PlayerId);
            stats.Property(s => s.MatchesPlayed).HasDefaultValue(0);
            stats.Property(s => s.MatchesWon).HasDefaultValue(0);
            stats.Property(s => s.TotalFloorsBuilt).HasDefaultValue(0);
            stats.Property(s => s.HighestSkyscraper).HasDefaultValue(0);
            stats.HasIndex(s => new { s.MatchesWon, s.TotalFloorsBuilt })
                .IsDescending(true, true).HasDatabaseName("IX_PlayerStats_Ranking");
            stats.HasOne(s => s.Player).WithOne(p => p.Stats)
                .HasForeignKey<PlayerStats>(s => s.PlayerId)
                .HasConstraintName("FK_PlayerStats_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSessionTokens(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SessionToken>(token =>
        {
            token.ToTable("SessionTokens");
            token.HasKey(t => t.TokenId);
            token.Property(t => t.TokenId).HasDefaultValueSql("NEWID()");
            token.Property(t => t.TokenValue).HasMaxLength(128).IsRequired();
            token.Property(t => t.ExpiresAt).HasColumnType("datetime2");
            token.Property(t => t.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("SYSUTCDATETIME()");
            token.Property(t => t.IsRevoked).HasDefaultValue(false);
            token.HasAlternateKey(t => t.TokenValue);
            token.HasIndex(t => t.TokenValue).HasDatabaseName("IX_SessionTokens_TokenValue");
            token.HasOne(t => t.Player).WithMany(p => p.SessionTokens)
                .HasForeignKey(t => t.PlayerId)
                .HasConstraintName("FK_SessionTokens_Players")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureFriendships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(friendship =>
        {
            friendship.ToTable("Friendships", table => table.HasCheckConstraint(
                "CK_Friendship_NotSelf", "[RequesterId] <> [AddresseeId]"));
            friendship.HasKey(f => f.FriendshipId);
            friendship.Property(f => f.FriendshipId).HasDefaultValueSql("NEWID()");
            friendship.Property(f => f.PlayerRequesterId).HasColumnName("RequesterId");
            friendship.Property(f => f.PlayerAddresseeId).HasColumnName("AddresseeId");
            friendship.Property(f => f.Status).HasConversion<byte>().HasColumnType("tinyint")
                .HasDefaultValue(FriendshipStatus.Pending);
            friendship.Property(f => f.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            friendship.Property(f => f.UpdatedAt).HasColumnType("datetime");
            friendship.HasAlternateKey(f => new { f.PlayerRequesterId, f.PlayerAddresseeId })
                .HasName("UQ_Friendship_Pair");
            friendship.HasOne(f => f.PlayerRequester).WithMany(p => p.SentFriendships)
                .HasForeignKey(f => f.PlayerRequesterId)
                .HasConstraintName("FK_Friendships_Requester")
                .OnDelete(DeleteBehavior.NoAction);
            friendship.HasOne(f => f.PlayerAddressee).WithMany(p => p.ReceivedFriendships)
                .HasForeignKey(f => f.PlayerAddresseeId)
                .HasConstraintName("FK_Friendships_Addressee")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureBlockedPlayers(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockedPlayer>(blocked =>
        {
            blocked.ToTable("BlockedPlayers", table => table.HasCheckConstraint(
                "CK_Blocked_NotSelf", "[BlockerId] <> [BlockedId]"));
            blocked.HasKey(b => b.BlockId);
            blocked.Property(b => b.BlockId).HasDefaultValueSql("NEWID()");
            blocked.Property(b => b.PlayerBlockerId).HasColumnName("BlockerId");
            blocked.Property(b => b.PlayerBlockedId).HasColumnName("BlockedId");
            blocked.Property(b => b.CreatedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            blocked.HasAlternateKey(b => new { b.PlayerBlockerId, b.PlayerBlockedId })
                .HasName("UQ_Blocked_Pair");
            blocked.HasOne(b => b.PlayerBlocker).WithMany(p => p.BlockedPlayers)
                .HasForeignKey(b => b.PlayerBlockerId)
                .HasConstraintName("FK_BlockedPlayers_Blocker")
                .OnDelete(DeleteBehavior.NoAction);
            blocked.HasOne(b => b.PlayerBlocked).WithMany(p => p.BlockedByPlayers)
                .HasForeignKey(b => b.PlayerBlockedId)
                .HasConstraintName("FK_BlockedPlayers_Blocked")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private static void ConfigureMatchHistory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MatchHistory>(match =>
        {
            match.ToTable("MatchHistory");
            match.HasKey(m => m.MatchId);
            match.Property(m => m.MatchId).HasDefaultValueSql("NEWID()");
            match.Property(m => m.PlayedAt).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            match.Property(m => m.RoundsPlayed).HasDefaultValue(3);
            match.Property(m => m.WinningHeight).HasDefaultValue(0);
            match.HasOne(m => m.Winner).WithMany(p => p.WonMatches)
                .HasForeignKey(m => m.WinnerId)
                .HasConstraintName("FK_MatchHistory_Winner")
                .OnDelete(DeleteBehavior.NoAction);
        });
    }

    private const string PlayerSchema = "dbo";
    private const string PlayerTable = "Players";
    private const string UsernameIndex = "IX_Players_Username";
    private const int LegacyUsernameLength = 32;
    private const int LegacyEmailLength = 120;

    private void ApplyExistingSchemaChanges()
    {
        var connection = Database.GetDbConnection();
        using DataTable columns = connection.GetSchema("Columns", [null, PlayerSchema, PlayerTable]);
        int? usernameLength = ReadColumnLength(columns, nameof(Player.Username), "nvarchar", nullable: false);
        int? emailLength = ReadColumnLength(columns, nameof(Player.Email), "nvarchar", nullable: false);
        int? countryLength = ReadColumnLength(columns, nameof(Player.CountryCode), "varchar", nullable: true);
        if (countryLength.HasValue && countryLength != CountryCodeLength)
        {
            throw new InvalidOperationException("The existing Players.CountryCode column has an unsupported length.");
        }

        MigrationBuilder migration = new(Database.ProviderName);
        bool reduceLengths = usernameLength != RegistrationRules.UsernameMaximumLength
            || emailLength != RegistrationRules.EmailMaximumLength;
        if (reduceLengths)
        {
            if (usernameLength != LegacyUsernameLength || emailLength != LegacyEmailLength)
            {
                throw new InvalidOperationException("Expected the legacy or current dbo.Players registration columns.");
            }

            using DataTable indexColumns = connection.GetSchema("IndexColumns", [null, PlayerSchema, PlayerTable]);
            Dictionary<string, string?> collations = Set<RegistrationColumn>()
                .Where(column => column.Schema == PlayerSchema && column.Table == PlayerTable
                    && (column.Name == nameof(Player.Username) || column.Name == nameof(Player.Email)))
                .ToDictionary(column => column.Name, column => column.Collation, StringComparer.OrdinalIgnoreCase);
            AddRegistrationLengthChanges(migration, indexColumns, collations);
        }

        if (!countryLength.HasValue)
        {
            migration.AddColumn<string>(nameof(Player.CountryCode), PlayerTable, schema: PlayerSchema,
                unicode: false, maxLength: CountryCodeLength, nullable: true);
        }

        if (migration.Operations.Count > 0)
        {
            using IDbContextTransaction transaction = Database.BeginTransaction(IsolationLevel.Serializable);
            if (reduceLengths && Players.Any(player =>
                EF.Functions.DataLength(player.Username) > RegistrationRules.UsernameMaximumLength * sizeof(char)
                || EF.Functions.DataLength(player.Email) > RegistrationRules.EmailMaximumLength * sizeof(char)))
            {
                throw new InvalidOperationException("Existing usernames or emails exceed the registration limits.");
            }

            var generator = this.GetService<IMigrationsSqlGenerator>();
            var relationalConnection = this.GetService<IRelationalConnection>();
            foreach (MigrationCommand command in generator.Generate(migration.Operations))
            {
                command.ExecuteNonQuery(relationalConnection, null);
            }

            transaction.Commit();
        }
    }

    private static int? ReadColumnLength(DataTable columns, string column, string storeType, bool nullable)
    {
        DataRow? row = columns.Rows.Cast<DataRow>()
            .SingleOrDefault(item => string.Equals(item.Field<string>("COLUMN_NAME"), column,
                StringComparison.OrdinalIgnoreCase));
        if (row is not null && (!string.Equals(row.Field<string>("DATA_TYPE"), storeType,
            StringComparison.OrdinalIgnoreCase) || (row.Field<string>("IS_NULLABLE") == "YES") != nullable))
        {
            throw new InvalidOperationException($"Unexpected definition for dbo.Players.{column}.");
        }

        return row?.Field<int?>("CHARACTER_MAXIMUM_LENGTH");
    }

    private static void AddRegistrationLengthChanges(MigrationBuilder migration, DataTable indexColumns,
        IReadOnlyDictionary<string, string?> collations)
    {
        Dictionary<string, string?[]> indexes = indexColumns.Rows.Cast<DataRow>()
            .GroupBy(row => row.Field<string>("index_name")!, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(row => row.Field<string>("column_name")).ToArray(),
                StringComparer.Ordinal);
        string usernameKey = FindRegistrationKey(indexes, nameof(Player.Username));
        string emailKey = FindRegistrationKey(indexes, nameof(Player.Email));
        bool hasUsernameIndex = indexes.TryGetValue(UsernameIndex, out string?[]? usernameIndexColumns);
        if (hasUsernameIndex)
        {
            if (!usernameIndexColumns!.SequenceEqual([nameof(Player.Username)], StringComparer.Ordinal))
            {
                throw new InvalidOperationException("Unexpected IX_Players_Username definition.");
            }

            migration.DropIndex(UsernameIndex, PlayerTable, PlayerSchema);
        }

        migration.DropUniqueConstraint(usernameKey, PlayerTable, PlayerSchema);
        migration.DropUniqueConstraint(emailKey, PlayerTable, PlayerSchema);
        AddRegistrationColumnChange(migration,
            (nameof(Player.Username), LegacyUsernameLength, RegistrationRules.UsernameMaximumLength), collations);
        AddRegistrationColumnChange(migration,
            (nameof(Player.Email), LegacyEmailLength, RegistrationRules.EmailMaximumLength), collations);
        migration.AddUniqueConstraint(usernameKey, PlayerTable, nameof(Player.Username), PlayerSchema);
        migration.AddUniqueConstraint(emailKey, PlayerTable, nameof(Player.Email), PlayerSchema);
        if (hasUsernameIndex)
        {
            migration.CreateIndex(UsernameIndex, PlayerTable, nameof(Player.Username), PlayerSchema);
        }
    }

    private static void AddRegistrationColumnChange(MigrationBuilder migration, (string Name, int OldLength, int Length) column,
        IReadOnlyDictionary<string, string?> collations)
    {
        string collation = collations.GetValueOrDefault(column.Name)
            ?? throw new InvalidOperationException($"Could not determine the collation for dbo.Players.{column.Name}.");
        migration.AlterColumn<string>(column.Name, PlayerTable, schema: PlayerSchema, unicode: true,
            maxLength: column.Length, collation: collation, oldClrType: typeof(string), oldUnicode: true,
            oldMaxLength: column.OldLength, oldCollation: collation);
    }

    private static string FindRegistrationKey(Dictionary<string, string?[]> indexes, string column)
    {
        string[] candidates = indexes.Where(index => index.Key != UsernameIndex
                && index.Value.Length == 1 && index.Value[0] == column)
            .Select(index => index.Key).ToArray();
        if (candidates.Length != 1)
        {
            throw new InvalidOperationException($"Expected a single unique constraint for dbo.Players.{column}.");
        }

        return candidates[0];
    }

    private sealed class RegistrationColumn
    {
        public string Schema { get; init; } = string.Empty;
        public string Table { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Collation { get; init; }
    }
}
