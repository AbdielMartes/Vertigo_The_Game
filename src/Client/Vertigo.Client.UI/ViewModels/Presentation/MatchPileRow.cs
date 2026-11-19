namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchPileRow
{
    public int Position { get; init; }
    public int? TopCardNumber { get; init; }
    public required string Label { get; init; }
    public string TopCardText { get; init; } = string.Empty;
    public int Count { get; init; }
}
