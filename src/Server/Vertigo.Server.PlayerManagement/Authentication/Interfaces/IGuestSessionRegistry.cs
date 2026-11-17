using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface IGuestSessionRegistry
{
    AuthenticatedSession Create();

    SessionIdentity? Find(string tokenHash);

    AuthenticatedSession? GetSession(string tokenHash);

    bool Revoke(string tokenHash);
}
