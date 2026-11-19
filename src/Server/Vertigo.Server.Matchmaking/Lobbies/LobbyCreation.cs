namespace Vertigo.Server.Matchmaking.Lobbies;

internal sealed record LobbyCreation(bool IsPublic, int MaxPlayers, DateTimeOffset CreatedAtUtc);
