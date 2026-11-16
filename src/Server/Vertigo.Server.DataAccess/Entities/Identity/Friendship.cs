using Vertigo.Server.DataAccess.Entities.Enums;

namespace Vertigo.Server.DataAccess.Entities.Identity;

public class Friendship
{
    public Guid FriendshipId { get; set; }
    public Guid PlayerRequesterId { get; set; }
    public Guid PlayerAddresseeId { get; set; }
    public FriendshipStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Player PlayerRequester { get; set; } = null!;
    public Player PlayerAddressee { get; set; } = null!;
}
