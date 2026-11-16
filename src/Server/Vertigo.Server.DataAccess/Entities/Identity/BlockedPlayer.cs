namespace Vertigo.Server.DataAccess.Entities.Identity;

public class BlockedPlayer
{
    public Guid BlockId { get; set; }
    public Guid PlayerBlockerId { get; set; }
    public Guid PlayerBlockedId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Player PlayerBlocker { get; set; } = null!;
    public Player PlayerBlocked { get; set; } = null!;
}
