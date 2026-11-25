using CoreWCF;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class AuthenticationService : IAuthService
{
    private const string EventCategoryProperty = "EventCategory";
    private const string SecurityCategory = "Security";
    private const string AuthenticationSucceededResult = "Succeeded";
    private const string AuthenticationFailedResult = "Failed";

    private static readonly EventId AuthenticationFailedEvent = new(2001, "AuthenticationFailed");
    private static readonly EventId AuthenticationSucceededEvent = new(2002, "AuthenticationSucceeded");

    private readonly SessionService _authentication;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(SessionService authentication, ILogger<AuthenticationService> logger)
    {
        _authentication = authentication;
        _logger = logger;
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
            LogAuthenticationResult(false, null);
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        LogAuthenticationResult(true, session.PlayerId);
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

    private void LogAuthenticationResult(bool isSucceeded, Guid? playerId)
    {
        EventId eventId = isSucceeded ? AuthenticationSucceededEvent : AuthenticationFailedEvent;
        string result = isSucceeded ? AuthenticationSucceededResult : AuthenticationFailedResult;
        Dictionary<string, object> properties = new()
        {
            [EventCategoryProperty] = SecurityCategory
        };

        using (IDisposable? scope = _logger.BeginScope(properties))
        {
            _logger.LogInformation(eventId, "Authentication {Result}. PlayerId: {PlayerId}", result, playerId);
        }
    }

    private static UserSession ConvertSession(AuthenticatedSession session)
    {
        return new UserSession
        {
            PlayerId = session.PlayerId,
            Username = session.Username,
            AvatarId = session.AvatarId,
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
