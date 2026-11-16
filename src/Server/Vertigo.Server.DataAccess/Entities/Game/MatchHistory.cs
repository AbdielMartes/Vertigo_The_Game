using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Entities.Game;

public class MatchHistory
{
    public Guid MatchId { get; set; }
    public DateTime PlayedAt { get; set; }
    public int RoundsPlayed { get; set; }
    public Guid? WinnerId { get; set; }
    public int WinningHeight { get; set; }

    public Player? Winner { get; set; }
}
