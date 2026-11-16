using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
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

    public AuthService(AuthenticationDependencies dependencies)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _tokenGenerator = dependencies.TokenGenerator;
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
        DateTime expiresAt = DateTime.UtcNow.AddDays(SessionDurationDays);
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
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return null;
        }

        string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
        SessionToken? storedToken = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
        if (storedToken is null)
        {
            return null;
        }

        return new AuthenticatedSession
        {
            PlayerId = storedToken.PlayerId,
            Username = storedToken.Player.Username,
            Token = sessionToken,
            ExpiresAt = storedToken.ExpiresAt
        };
    }

    public bool Logout(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
        {
            return false;
        }

        string tokenHash = _tokenGenerator.HashSessionToken(sessionToken);
        bool isRevoked = _sessionTokenRepository.RevokeSessionToken(tokenHash);

        return isRevoked;
    }

    public AuthenticatedSession CreateGuestSession()
    {
        string guestCode = _tokenGenerator.GenerateVerificationCode();
        AuthenticatedSession guestSession = new AuthenticatedSession
        {
            PlayerId = Guid.NewGuid(),
            Username = $"Guest_{guestCode}",
            IsGuest = true
        };

        return guestSession;
    }
}
