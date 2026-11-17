using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface ISessionIdentityResolver
{
    SessionIdentity? Resolve(string token);

    SessionIdentity Require(string token);

    SessionIdentity RequireRegistered(string token);
}
