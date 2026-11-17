namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed record LobbySettings(
    string LobbyCode,
    Guid HostPlayerId,
    bool IsPublic,
    int MaxPlayers,
    DateTimeOffset CreatedAtUtc);
