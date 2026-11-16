namespace Vertigo.Server.DataAccess.Entities.Identity;

public class PlayerStats
{
    public Guid PlayerId { get; set; }
    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int TotalFloorsBuilt { get; set; }
    public int HighestSkyscraper { get; set; }

    public Player Player { get; set; } = null!;
}
