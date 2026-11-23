namespace Vertigo.Contracts.Internal;

public sealed record CompletedMatch(Guid MatchId, DateTime CompletedAt, int Rounds, int WinningHeight,
    Guid? SoleWinner, IReadOnlyList<CompletedMatchPlayer> Players);

public sealed record CompletedMatchPlayer(Guid PlayerId, bool Won, int FloorsBuilt, int HighestSkyscraper);
