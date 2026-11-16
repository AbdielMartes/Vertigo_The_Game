namespace Vertigo.Server.DataAccess.Entities.Identity;

public class SessionToken
{
    public Guid TokenId { get; set; }
    public Guid PlayerId { get; set; }
    public string TokenValue { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }

    public Player Player { get; set; } = null!;
}
