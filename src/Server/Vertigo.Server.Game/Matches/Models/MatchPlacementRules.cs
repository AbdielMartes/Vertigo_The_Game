namespace Vertigo.Server.Game.Matches.Models;

public static class MatchPlacementRules
{
    public static bool IsValid(int currentTop, int candidate)
    {
        return currentTop is >= 1 and <= 100 && candidate is >= 1 and <= 100
            && candidate > currentTop && candidate - currentTop <= 10;
    }
}
