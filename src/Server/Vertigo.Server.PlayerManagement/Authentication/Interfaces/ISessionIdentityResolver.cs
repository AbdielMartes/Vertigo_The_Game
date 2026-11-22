using System.Runtime.CompilerServices;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Interfaces;

public interface ISessionIdentityResolver
{
    SessionIdentity? Resolve(string? token);

    SessionIdentity Require(string? token, [CallerMemberName] string operation = "");

    SessionIdentity RequireRegistered(string? token, [CallerMemberName] string operation = "");
}
