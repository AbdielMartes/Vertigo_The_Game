namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed class AuthenticatedSession
{
    public Guid PlayerId { get; init; }

    public string Username { get; init; } = string.Empty;

    public int? AvatarId { get; init; }

    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }

    public bool IsGuest { get; init; }
}
