using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Sessions.Interfaces;

public interface ISessionCoordinator
{
    ClientResult<UserSession> Login(string username, string password);
    ClientResult<UserSession> ResumeSession(string token);
    ClientResult<UserSession> CreateGuestSession();
    ClientResult<bool> Logout();
}
