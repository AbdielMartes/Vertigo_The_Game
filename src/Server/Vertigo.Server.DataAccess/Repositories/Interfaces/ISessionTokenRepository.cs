using Vertigo.Server.DataAccess.Entities.Identity;

namespace Vertigo.Server.DataAccess.Repositories.Interfaces;

public interface ISessionTokenRepository
{
    bool CreateSessionToken(SessionToken token);

    SessionToken? GetActiveSessionToken(string tokenHash);

    bool RevokeSessionToken(string tokenHash);

    int RevokePlayerSessions(Guid playerId);
}
