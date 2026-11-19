using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class RegisteredPlayerSessions
{
    private const int SessionDurationDays = 7;

    private readonly ISessionTokenRepository _sessionTokens;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public RegisteredPlayerSessions(
        ISessionTokenRepository sessionTokens,
        ITokenGenerator tokenGenerator,
        TimeProvider timeProvider)
    {
        _sessionTokens = sessionTokens;
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider;
    }

    internal AuthenticatedSession Create(Player player)
    {
        string sessionToken = _tokenGenerator.GenerateSessionToken();
        DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
        DateTime expiresAt = now.AddDays(SessionDurationDays);
        SessionToken token = new()
        {
            PlayerId = player.PlayerId,
            TokenValue = HashToken(sessionToken),
            ExpiresAt = expiresAt
        };

        bool isCreated = _sessionTokens.CreateSessionToken(token);
        if (!isCreated)
        {
            throw new InvalidOperationException("The session could not be persisted.");
        }

        return new AuthenticatedSession
        {
            PlayerId = player.PlayerId,
            Username = player.Username,
            Token = sessionToken,
            ExpiresAt = expiresAt
        };
    }

    internal AuthenticatedSession? Resume(string sessionToken, string tokenHash)
    {
        SessionToken? storedToken = _sessionTokens.GetActiveSessionToken(tokenHash);
        AuthenticatedSession? session = null;
        if (storedToken is not null)
        {
            session = new AuthenticatedSession
            {
                PlayerId = storedToken.PlayerId,
                Username = storedToken.Player.Username,
                Token = sessionToken,
                ExpiresAt = storedToken.ExpiresAt
            };
        }

        return session;
    }

    internal string HashToken(string sessionToken)
    {
        return _tokenGenerator.HashSessionToken(sessionToken);
    }

    internal bool Revoke(string tokenHash)
    {
        return _sessionTokens.RevokeSessionToken(tokenHash);
    }
}
