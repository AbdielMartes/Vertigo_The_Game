using Vertigo.Server.DataAccess.Entities.Game;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Player
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public int? AvatarId { get; set; }
    public string? CountryCode { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public PlayerStats? Stats { get; set; }
    public ICollection<SessionToken> SessionTokens { get; set; } = new List<SessionToken>();
    public ICollection<Friendship> SentFriendships { get; set; } = new List<Friendship>();
    public ICollection<Friendship> ReceivedFriendships { get; set; } = new List<Friendship>();
    public ICollection<BlockedPlayer> BlockedPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<BlockedPlayer> BlockedByPlayers { get; set; } = new List<BlockedPlayer>();
    public ICollection<MatchHistory> WonMatches { get; set; } = new List<MatchHistory>();
}
