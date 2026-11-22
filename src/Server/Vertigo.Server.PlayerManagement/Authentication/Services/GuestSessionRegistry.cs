using Vertigo.Server.CrossCutting.Recovery;
using System.Collections.Concurrent;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class GuestSessionRegistry : IGuestSessionRegistry
{
    private const int GuestDurationHours = 12;
    private sealed record GuestCredential(Guid PlayerId, string Username, DateTime ExpiresAt);
    private readonly string? _directory;
    private readonly object _writeLock = new();

    private readonly ConcurrentDictionary<string, AuthenticatedSession> _sessions = new();
    private readonly ITokenGenerator _tokenGenerator;
    private readonly TimeProvider _timeProvider;

    public GuestSessionRegistry(ITokenGenerator tokenGenerator, TimeProvider timeProvider,
        RecoveryDirectory? recovery = null)
    {
        ArgumentNullException.ThrowIfNull(tokenGenerator);
        ArgumentNullException.ThrowIfNull(timeProvider);
        _tokenGenerator = tokenGenerator;
        _timeProvider = timeProvider;
        if (recovery is not null)
        {
            _directory = Path.Combine(recovery.Root, "guests-v1");
            Directory.CreateDirectory(_directory);
            RestoreGuests();
        }
    }

    private void RestoreGuests()
    {
        foreach (string file in Directory.EnumerateFiles(_directory!, "*.json"))
        {
            GuestCredential credential = RecoveryDirectory.Read<GuestCredential>(file);
            if (credential.ExpiresAt > _timeProvider.GetUtcNow().UtcDateTime)
            {
                _sessions.TryAdd(Path.GetFileNameWithoutExtension(file), new AuthenticatedSession
                {
                    PlayerId = credential.PlayerId,
                    Username = credential.Username,
                    ExpiresAt = credential.ExpiresAt,
                    IsGuest = true
                });
            }
            else
            {
                File.Delete(file);
            }
        }
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
        lock (_writeLock)
        {
            if (_sessions.ContainsKey(tokenHash))
            {
                throw new InvalidOperationException("A guest session could not be registered.");
            }

            if (_directory is not null)
            {
                RecoveryDirectory.Write(Path.Combine(_directory, tokenHash + ".json"),
                    new GuestCredential(session.PlayerId, session.Username, session.ExpiresAt));
            }

            _sessions[tokenHash] = session;
        }

        return session;
    }

    public SessionIdentity? Find(string tokenHash)
    {
        AuthenticatedSession? session = GetSession(tokenHash);
        return session is null ? null : new SessionIdentity(session.PlayerId, session.Username, true);
    }

    public AuthenticatedSession? GetSession(string tokenHash)
    {
        if (_sessions.TryGetValue(tokenHash, out AuthenticatedSession? session)
            && session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            _sessions.TryRemove(new KeyValuePair<string, AuthenticatedSession>(tokenHash, session));
            session = null;
        }

        return session;
    }

    public bool Revoke(string tokenHash)
    {
        bool revoked;
        lock (_writeLock)
        {
            if (_directory is not null && _sessions.ContainsKey(tokenHash))
            {
                File.Delete(Path.Combine(_directory, tokenHash + ".json"));
            }

            revoked = _sessions.TryRemove(tokenHash, out _);
        }

        return revoked;
    }
}
