using CommunityToolkit.Mvvm.ComponentModel;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchCardRow : ObservableObject
{
    private int? _planOrder;

    public int? CardNumber { get; init; }
    public required string DisplayValue { get; init; }
    public bool IsWildcard { get; init; }

    public int? PlanOrder
    {
        get => _planOrder;
        internal set
        {
            if (SetProperty(ref _planOrder, value))
            {
                OnPropertyChanged(nameof(IsInMasterPlan));
            }
        }
    }

    public bool IsInMasterPlan => PlanOrder.HasValue;
}
