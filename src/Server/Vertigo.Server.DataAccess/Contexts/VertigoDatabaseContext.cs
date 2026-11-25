using Microsoft.EntityFrameworkCore;
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

        ApplySchemaScript("001_ReducePlayerRegistrationLengths.sql");
        ApplySchemaScript("002_AddPlayerCountryCode.sql");
    }

    private void ApplySchemaScript(string fileName)
    {
        string resource = "Vertigo.Server.DataAccess.Sql." + fileName;
        using Stream script = typeof(VertigoDatabaseContext).Assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"Schema script {fileName} is missing.");
        using StreamReader reader = new(script);
        Database.ExecuteSqlRaw(reader.ReadToEnd());
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
}
