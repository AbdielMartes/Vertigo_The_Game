using System.Collections.Concurrent;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class GuestSessionRegistry : IGuestSessionRegistry
{
    private const int GuestDurationHours = 12;

    private readonly ConcurrentDictionary<string, AuthenticatedSession> _sessions = new();
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public GuestSessionRegistry(ITokenGenerator tokenGenerator, TimeProvider timeProvider)
    {
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider;
    }

    public AuthenticatedSession Create()
    {
        string token = _tokenGenerator.GenerateSessionToken();
        string guestCode = _tokenGenerator.GenerateVerificationCode();
        AuthenticatedSession session = new AuthenticatedSession
        {
            PlayerId = Guid.NewGuid(),
            Username = $"Guest_{guestCode}",
            Token = token,
            ExpiresAt = _timeProvider.GetUtcNow().UtcDateTime.AddHours(GuestDurationHours),
            IsGuest = true
        };

        string tokenHash = _tokenGenerator.HashSessionToken(token);
        if (!_sessions.TryAdd(tokenHash, session))
        {
            throw new InvalidOperationException("A guest session could not be registered.");
        }

        return session;
    }

    public SessionIdentity? Find(string tokenHash)
    {
        if (!_sessions.TryGetValue(tokenHash, out AuthenticatedSession? session))
        {
            return null;
        }

        if (session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            _sessions.TryRemove(tokenHash, out _);
            return null;
        }

        return new SessionIdentity(session.PlayerId, session.Username, true);
    }

    public AuthenticatedSession? GetSession(string tokenHash)
    {
        if (Find(tokenHash) is null)
        {
            return null;
        }

        return _sessions.TryGetValue(tokenHash, out AuthenticatedSession? session) ? session : null;
    }

    public bool Revoke(string tokenHash)
    {
        return _sessions.TryRemove(tokenHash, out _);
    }
}
