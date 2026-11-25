namespace Vertigo.Client.Core.Sessions;

public sealed record SessionSnapshot
{
    public required Guid PlayerId { get; init; }

    public required string Username { get; init; }

    public int? AvatarId { get; init; }

    public required string Token { get; init; }

    public required DateTime ExpiresAt { get; init; }

    public required bool IsGuest { get; init; }

    public override string ToString()
    {
        return nameof(SessionSnapshot);
    }
}
