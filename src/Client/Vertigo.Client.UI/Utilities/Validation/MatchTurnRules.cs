using System.Collections.Generic;
using System.Linq;

namespace Vertigo.Client.UI.Utilities.Validation;

internal static class MatchTurnRules
{
    private const int MaximumModule = 100;
    private const int MaximumRise = 10;

    public static bool HasLegalPlacement(IEnumerable<int?> tops, IReadOnlyList<int> hand)
    {
        return tops.Any(top => hand.Any(module => CanPlace(top, module)));
    }

    public static bool CanPlace(int? top, int? module)
    {
        return top is int foundation && module is int candidate
            && foundation is >= 1 and <= MaximumModule && candidate is >= 1 and <= MaximumModule
            && candidate - foundation is >= 1 and <= MaximumRise;
    }
}
