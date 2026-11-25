namespace Vertigo.Client.Core.Sessions;

public sealed class PlayerSession
{
    private readonly object _syncRoot = new();
    private readonly TimeProvider _timeProvider;
    private SessionSnapshot? _session;

    public PlayerSession(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public bool IsAuthenticated => GetActiveSession() is not null;

    public SessionSnapshot? GetActiveSession()
    {
        lock (_syncRoot)
        {
            if (_session is not null && _session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
            {
                _session = null;
            }

            return _session;
        }
    }

    public void SetSession(SessionSnapshot session)
    {
        ArgumentNullException.ThrowIfNull(session);
        if (session.PlayerId == Guid.Empty || string.IsNullOrWhiteSpace(session.Username)
            || string.IsNullOrWhiteSpace(session.Token) || session.ExpiresAt <= _timeProvider.GetUtcNow().UtcDateTime)
        {
            throw new ArgumentException("Session data is incomplete or expired.", nameof(session));
        }

        lock (_syncRoot)
        {
            _session = session;
        }
    }

    public bool UpdateProfile(SessionSnapshot expected, string username, int? avatarId)
    {
        ArgumentNullException.ThrowIfNull(expected);
        bool updated = false;
        lock (_syncRoot)
        {
            if (ReferenceEquals(GetActiveSession(), expected))
            {
                _session = expected with { Username = username, AvatarId = avatarId };
                updated = true;
            }
        }

        return updated;
    }

    public void Clear(SessionSnapshot session)
    {
        lock (_syncRoot)
        {
            if (ReferenceEquals(_session, session))
            {
                _session = null;
            }
        }
    }
}
