using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public class AuthenticationService : IAuthService
{
    private readonly AuthService _authentication;

    public AuthenticationService(AuthService authentication)
    {
        _authentication = authentication;
    }

    public UserSession Login(LoginCredentials credentials)
    {
        if (credentials is null || string.IsNullOrWhiteSpace(credentials.Username)
            || string.IsNullOrWhiteSpace(credentials.Password))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        AuthenticatedSession? session = _authentication.Login(credentials.Username, credentials.Password);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession ResumeSession(string token)
    {
        AuthenticatedSession? session = _authentication.ResumeSession(token);
        if (session is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        return ConvertSession(session);
    }

    public UserSession CreateGuestSession()
    {
        return ConvertSession(_authentication.CreateGuestSession());
    }

    public bool Logout(SessionLogout logout)
    {
        return logout is not null && _authentication.Logout(logout.SessionToken);
    }

    private static UserSession ConvertSession(AuthenticatedSession session)
    {
        return new UserSession
        {
            PlayerId = session.PlayerId,
            Username = session.Username,
            Token = session.Token,
            ExpiresAt = session.ExpiresAt,
            IsGuest = session.IsGuest
        };
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
