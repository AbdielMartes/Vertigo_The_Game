using CoreWCF;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class SessionIdentityResolver : ISessionIdentityResolver
{
    private const int SessionTokenLength = 64;
    private const string EventCategoryProperty = "EventCategory";
    private const string SecurityCategory = "Security";
    private const string FailedResult = "Failed";
    private const string DeniedResult = "Denied";

    private static readonly EventId SessionValidationFailedEvent = new(2003, "SessionValidationFailed");
    private static readonly EventId UnauthorizedAccessEvent = new(2005, "UnauthorizedAccess");

    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;
    private readonly ILogger<SessionIdentityResolver> _logger;

    public SessionIdentityResolver(
        SessionIdentityResolverDependencies dependencies,
        ILogger<SessionIdentityResolver> logger)
    {
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _tokenGenerator = dependencies.TokenGenerator;
        _guests = dependencies.Guests;
        _logger = logger;
    }

    public SessionIdentity? Resolve(string token)
    {
        SessionIdentity? identity = null;
        if (!string.IsNullOrWhiteSpace(token) && token.Length == SessionTokenLength)
        {
            string tokenHash = _tokenGenerator.HashSessionToken(token);
            identity = _guests.Find(tokenHash);
            if (identity is null)
            {
                SessionToken? session = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (session is not null)
                {
                    identity = new SessionIdentity(session.PlayerId, session.Player.Username, false);
                }
            }
        }

        return identity;
    }

    public SessionIdentity Require(string token)
    {
        SessionIdentity? identity = Resolve(token);
        if (identity is null)
        {
            LogSecurityEvent(SessionValidationFailedEvent, FailedResult, null);
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    public SessionIdentity RequireRegistered(string token)
    {
        SessionIdentity identity = Require(token);
        if (identity.IsGuest)
        {
            LogSecurityEvent(UnauthorizedAccessEvent, DeniedResult, identity.PlayerId);
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    private void LogSecurityEvent(EventId eventId, string result, Guid? playerId)
    {
        Dictionary<string, object> properties = new()
        {
            [EventCategoryProperty] = SecurityCategory
        };

        using (IDisposable? scope = _logger.BeginScope(properties))
        {
            _logger.LogWarning(eventId, "Session access {Result}. PlayerId: {PlayerId}", result, playerId);
        }
    }

    private static FaultException<ServiceFault> CreateUnauthorizedFault()
    {
        ServiceFault fault = new ServiceFault
        {
            ErrorCode = ServiceErrorCodes.Unauthorized,
            MessageKey = ServiceMessageKeys.Unauthorized
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(fault.ErrorCode));
    }
}
