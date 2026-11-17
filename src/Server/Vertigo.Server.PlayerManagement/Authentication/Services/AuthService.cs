using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthService
{
    private const int SessionDurationDays = 7;

    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;
    private readonly TimeProvider _timeProvider;

    public AuthService(AuthenticationDependencies dependencies, IGuestSessionRegistry guests, TimeProvider timeProvider)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _tokenGenerator = dependencies.TokenGenerator;
        _guests = guests;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession? Login(string username, string password)
    {
        Player? player = _playerRepository.GetPlayerByUsername(username);
        if (player is null || !_passwordHasher.VerifyPassword(
                password, player.PasswordHash, player.PasswordSalt))
        {
            return null;
        }

        string sessionToken = _tokenGenerator.GenerateSessionToken();
        DateTime expiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddDays(SessionDurationDays);
        SessionToken token = new SessionToken
        {
            PlayerId = player.PlayerId,
            TokenValue = _tokenGenerator.HashSessionToken(sessionToken),
            ExpiresAt = expiresAt
        };

        bool isCreated = _sessionTokenRepository.CreateSessionToken(token);
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

    public AuthenticatedSession? ResumeSession(string sessionToken)
    {
        AuthenticatedSession? session = null;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            session = _guests.GetSession(tokenHash);
            if (session is null)
            {
                SessionToken? storedToken = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
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
            }
        }

        return session;
    }

    public bool Logout(string sessionToken)
    {
        bool isRevoked = false;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
            isRevoked = _guests.Revoke(tokenHash) || _sessionTokenRepository.RevokeSessionToken(tokenHash);
        }

        return isRevoked;
    }

    public AuthenticatedSession CreateGuestSession()
    {
        return _guests.Create();
    }
}
