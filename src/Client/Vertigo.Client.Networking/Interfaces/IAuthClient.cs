using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IAuthClient
{
    ClientResult<UserSession> Login(LoginCredentials request);

    ClientResult<UserSession> ResumeSession(string sessionToken);

    ClientResult<UserSession> CreateGuestSession();

    ClientResult<bool> Logout(string sessionToken);
}
