namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchCardRow
{
    public int? CardNumber { get; init; }
    public required string DisplayValue { get; init; }
    public bool IsWildcard { get; init; }
}
