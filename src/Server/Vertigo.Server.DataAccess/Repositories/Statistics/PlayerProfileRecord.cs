namespace Vertigo.Server.DataAccess.Repositories.Statistics;

public sealed record PlayerProfileRecord(string Username, int CreatedYear, int? AvatarId, string? CountryCode,
    int MatchesPlayed, int MatchesWon);
