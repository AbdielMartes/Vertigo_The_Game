using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthService
{
    private readonly PlayerCredentialsVerifier _credentials;
    private readonly RegisteredPlayerSessions _registeredSessions;
    private readonly IGuestSessionRegistry _guests;

    public AuthService(
        PlayerCredentialsVerifier credentials,
        RegisteredPlayerSessions registeredSessions,
        IGuestSessionRegistry guests)
    {
        _credentials = credentials;
        _registeredSessions = registeredSessions;
        _guests = guests;
    }

    public AuthenticatedSession? Login(string username, string password)
    {
        AuthenticatedSession? session = null;
        Player? player = _credentials.FindVerifiedPlayer(username, password);
        if (player is not null)
        {
            session = _registeredSessions.Create(player);
        }

        return session;
    }

    public AuthenticatedSession? ResumeSession(string sessionToken)
    {
        AuthenticatedSession? session = null;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _registeredSessions.HashToken(sessionToken);
            session = _guests.GetSession(tokenHash);
            if (session is null)
            {
                session = _registeredSessions.Resume(sessionToken, tokenHash);
            }
        }

        return session;
    }

    public bool Logout(string sessionToken)
    {
        bool isRevoked = false;
        if (!string.IsNullOrWhiteSpace(sessionToken))
        {
            string tokenHash = _registeredSessions.HashToken(sessionToken);
            isRevoked = _guests.Revoke(tokenHash) || _registeredSessions.Revoke(tokenHash);
        }

        return isRevoked;
    }

    public AuthenticatedSession CreateGuestSession()
    {
        return _guests.Create();
    }
}
