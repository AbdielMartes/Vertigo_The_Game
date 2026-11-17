using Microsoft.EntityFrameworkCore;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.DataAccess.Repositories.Implementations;

public class SessionTokenRepository : ISessionTokenRepository
{
    private const int ExpectedAffectedRows = 1;

    private readonly VertigoDatabaseContext _context;

    public SessionTokenRepository(VertigoDatabaseContext context)
    {
        _context = context;
    }

    public bool CreateSessionToken(SessionToken token)
    {
        _context.SessionTokens.Add(token);
        int affectedRows = _context.SaveChanges();

        return affectedRows == ExpectedAffectedRows;
    }

    public SessionToken? GetActiveSessionToken(string tokenHash)
    {
        DateTime currentTime = DateTime.UtcNow;
        SessionToken? token = _context.SessionTokens
            .AsNoTracking()
            .Include(session => session.Player)
            .FirstOrDefault(session => session.TokenValue == tokenHash
                                       && !session.IsRevoked
                                       && session.ExpiresAt > currentTime);

        return token;
    }

    public bool RevokeSessionToken(string tokenHash)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.TokenValue == tokenHash && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows == ExpectedAffectedRows;
    }

    public int RevokePlayerSessions(Guid playerId)
    {
        int affectedRows = _context.SessionTokens
            .Where(session => session.PlayerId == playerId && !session.IsRevoked)
            .ExecuteUpdate(setters => setters.SetProperty(session => session.IsRevoked, true));

        return affectedRows;
    }

}
