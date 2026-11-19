namespace Vertigo.Contracts.Models;

public sealed class MatchBoardChanged
{
    public required string LobbyCode { get; init; }
    public Guid MatchId { get; init; }
    public long Revision { get; init; }
    public required string PlayerUsername { get; init; }
    public MatchBoardActionKind ActionKind { get; init; }
    public int CardNumber { get; init; }
    public int PilePosition { get; init; }
}
