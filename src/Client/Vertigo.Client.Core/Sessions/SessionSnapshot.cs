namespace Vertigo.Client.Core.Sessions;

public sealed record SessionSnapshot(
    Guid PlayerId,
    string Username,
    string Token,
    DateTime ExpiresAt,
    bool IsGuest)
{
    public override string ToString()
    {
        return nameof(SessionSnapshot);
    }
}
