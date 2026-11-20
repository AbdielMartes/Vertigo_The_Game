namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record MatchFinalStandingRow
{
    public int? Position { get; init; }
    public required string Username { get; init; }
    public int TotalFloors { get; init; }
    public bool IsWinner { get; init; }
}
