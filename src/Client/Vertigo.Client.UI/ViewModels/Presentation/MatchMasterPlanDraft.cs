using System.Collections.Generic;
using System.Linq;
using Vertigo.Client.UI.Utilities.Validation;

namespace Vertigo.Client.UI.ViewModels.Presentation;

internal sealed class MatchMasterPlanDraft
{
    private readonly List<int> _modules = new();

    public IReadOnlyList<int> Modules => _modules;

    public bool CanPlay => _modules.Count >= 2;

    public int? GetOrder(int? module)
    {
        int index = module is int number ? _modules.IndexOf(number) : -1;
        return index >= 0 ? index + 1 : null;
    }

    public bool CanAppend(int? towerTop, int? module)
    {
        int? top = _modules.Count == 0 ? towerTop : _modules[^1];

        return towerTop is not null && module is int card && !_modules.Contains(card)
            && MatchTurnRules.CanPlace(top, card);
    }

    public bool TryAppend(int? towerTop, int module)
    {
        bool added = CanAppend(towerTop, module);
        if (added)
        {
            _modules.Add(module);
        }

        return added;
    }

    public bool RemoveLast()
    {
        bool removed = _modules.Count > 0;
        if (removed)
        {
            _modules.RemoveAt(_modules.Count - 1);
        }

        return removed;
    }

    public bool IsStillInHand(IReadOnlyList<int> hand)
    {
        return _modules.All(module => hand.Contains(module));
    }

    public void Clear()
    {
        _modules.Clear();
    }
}
