using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Utilities;

public sealed class SocialOperation
{
    private readonly PlayerSession _session;
    private readonly ISessionGuard _guard;
    private readonly IClientErrorText _errors;

    public SocialOperation(PlayerSession session, ISessionGuard guard, IClientErrorText errors)
    {
        _session = session;
        _guard = guard;
        _errors = errors;
    }

    public SessionSnapshot? GetRegisteredSession()
    {
        SessionSnapshot? session = _session.GetActiveSession();
        if (session is null)
        {
            _guard.Invalidate();
            return null;
        }
        return session.IsGuest ? null : session;
    }

    public bool IsCurrent(SessionSnapshot original)
    {
        return _guard.IsCurrent(original);
    }

    public bool Check<T>(ClientResult<T> result, SessionSnapshot original)
    {
        if (!_guard.IsCurrent(original))
        {
            return false;
        }
        if (_errors.IsUnauthorized(result))
        {
            _guard.Invalidate(original);
            return false;
        }
        return true;
    }

    public string Error<T>(ClientResult<T> result, string fallbackKey)
    {
        return _errors.FromResult(result, fallbackKey);
    }
}
