namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchPlayerRow
{
    public required string Username { get; init; }
    public int FloorsBuilt { get; init; }
    public bool IsCurrentTurn { get; init; }
    public bool IsMe { get; init; }
}
