using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;

namespace Vertigo.Client.Networking.Proxies;

public sealed class AuthClient : IAuthClient
{
    private readonly IServiceCaller _serviceCaller;

    public AuthClient(IServiceCaller serviceCaller)
    {
        _serviceCaller = serviceCaller;
    }

    public ClientResult<UserSession> Login(LoginCredentials request)
    {
        return _serviceCaller.Call<IAuthService, LoginCredentials, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            LoginOperation,
            request);
    }

    public ClientResult<UserSession> ResumeSession(string sessionToken)
    {
        return _serviceCaller.Call<IAuthService, string, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            ResumeSessionOperation,
            sessionToken);
    }

    public ClientResult<UserSession> CreateGuestSession()
    {
        return _serviceCaller.Call<IAuthService, object?, UserSession>(
            ServiceEndpoints.AuthServiceAddress,
            CreateGuestSessionOperation,
            null);
    }

    public ClientResult<bool> Logout(string sessionToken)
    {
        SessionLogout request = new()
        {
            SessionToken = sessionToken
        };

        return _serviceCaller.Call<IAuthService, SessionLogout, bool>(
            ServiceEndpoints.AuthServiceAddress,
            LogoutOperation,
            request);
    }

    private static UserSession LoginOperation(IAuthService channel, LoginCredentials request)
    {
        return channel.Login(request);
    }

    private static UserSession ResumeSessionOperation(IAuthService channel, string sessionToken)
    {
        return channel.ResumeSession(sessionToken);
    }

    private static UserSession CreateGuestSessionOperation(IAuthService channel, object? unused)
    {
        return channel.CreateGuestSession();
    }

    private static bool LogoutOperation(IAuthService channel, SessionLogout request)
    {
        return channel.Logout(request);
    }
}
