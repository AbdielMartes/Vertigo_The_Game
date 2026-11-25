using System.Runtime.CompilerServices;
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
        ArgumentNullException.ThrowIfNull(dependencies);
        ArgumentNullException.ThrowIfNull(logger);
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _tokenGenerator = dependencies.TokenGenerator;
        _guests = dependencies.Guests;
        _logger = logger;
    }

    public SessionIdentity? Resolve(string? token)
    {
        return Resolve(token, out _);
    }

    private SessionIdentity? Resolve(string? token, out string rejection)
    {
        SessionIdentity? identity = null;
        if (string.IsNullOrWhiteSpace(token))
        {
            rejection = "MissingToken";
        }
        else if (token.Length != SessionTokenLength || !token.All(char.IsAsciiHexDigit))
        {
            rejection = "MalformedToken";
        }
        else
        {
            rejection = "UnknownOrInactiveToken";
            string tokenHash = _tokenGenerator.HashSessionToken(token);
            identity = _guests.Find(tokenHash);
            if (identity is null)
            {
                SessionToken? session = _sessionTokenRepository.GetActiveSessionToken(tokenHash);
                if (session is not null)
                {
                    identity = new SessionIdentity(session.PlayerId, session.Player.Username, false)
                    {
                        AvatarId = session.Player.AvatarId
                    };
                }
            }
        }

        return identity;
    }

    public SessionIdentity Require(string? token, [CallerMemberName] string operation = "")
    {
        SessionIdentity? identity = Resolve(token, out string rejection);
        if (identity is null)
        {
            LogSecurityEvent(SessionValidationFailedEvent, operation, (FailedResult, rejection, null));
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    public SessionIdentity RequireRegistered(string? token, [CallerMemberName] string operation = "")
    {
        SessionIdentity identity = Require(token, operation);
        if (identity.IsGuest)
        {
            LogSecurityEvent(UnauthorizedAccessEvent, operation, (DeniedResult, "RegisteredPlayerRequired", identity.PlayerId));
            throw CreateUnauthorizedFault();
        }

        return identity;
    }

    private void LogSecurityEvent(EventId eventId, string operation,
        (string Result, string Reason, Guid? PlayerId) rejection)
    {
        Dictionary<string, object> properties = new()
        {
            [EventCategoryProperty] = SecurityCategory
        };

        using (IDisposable? scope = _logger.BeginScope(properties))
        {
            _logger.LogWarning(eventId, "Session access {Result} for {Operation}. Reason: {Reason}. PlayerId: {PlayerId}",
                rejection.Result, operation, rejection.Reason, rejection.PlayerId);
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
