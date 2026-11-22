using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.Utilities.Sessions.Interfaces;

public interface ISessionGuard
{
    bool HasActiveSession();
    bool IsCurrent(SessionSnapshot session);
    void Invalidate(SessionSnapshot? session = null, string? messageKey = null);
    string? TakeInvalidationMessage();
}
