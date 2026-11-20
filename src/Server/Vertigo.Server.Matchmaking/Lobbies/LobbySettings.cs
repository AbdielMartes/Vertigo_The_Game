namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed record LobbySettings
{
    public required string LobbyCode { get; init; }

    public required Guid HostPlayerId { get; init; }

    public required bool IsPublic { get; init; }

    public required int MaxPlayers { get; init; }

    public required DateTimeOffset CreatedAtUtc { get; init; }
}
