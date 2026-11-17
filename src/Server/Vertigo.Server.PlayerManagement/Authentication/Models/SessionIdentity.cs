namespace Vertigo.Server.PlayerManagement.Authentication.Models;

public sealed record SessionIdentity(Guid PlayerId, string Username, bool IsGuest);
