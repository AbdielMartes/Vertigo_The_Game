using CoreWCF;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public sealed class AccountPasswordService
{
    private const string EventCategoryProperty = "EventCategory";
    private const string SecurityCategory = "Security";
    private const string SucceededResult = "Succeeded";
    private const string RejectedResult = "Rejected";

    private static readonly EventId PasswordChangeSucceededEvent = new(2101, "PasswordChangeSucceeded");
    private static readonly EventId PasswordChangeRejectedEvent = new(2102, "PasswordChangeRejected");

    private readonly AccountPasswordStore _passwordStore;
    private readonly ISessionIdentityResolver _sessions;
    private readonly ILogger<AccountService> _logger;

    public AccountPasswordService(
        AccountPasswordStore passwordStore,
        ISessionIdentityResolver sessions,
        ILogger<AccountService> logger)
    {
        _passwordStore = passwordStore;
        _sessions = sessions;
        _logger = logger;
    }

    public bool ChangePassword(PasswordChange change, string sessionToken)
    {
        if (change is null || string.IsNullOrWhiteSpace(change.Username)
            || string.IsNullOrWhiteSpace(change.CurrentPassword)
            || string.IsNullOrWhiteSpace(change.NewPassword))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        PasswordChangeOutcome outcome = _passwordStore.TryChange(identity.PlayerId, change);
        if (outcome == PasswordChangeOutcome.InvalidCredentials)
        {
            LogPasswordChange(PasswordChangeRejectedEvent, RejectedResult, identity.PlayerId);
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        bool isChanged = outcome == PasswordChangeOutcome.Updated;
        if (isChanged)
        {
            LogPasswordChange(PasswordChangeSucceededEvent, SucceededResult, identity.PlayerId);
        }

        return isChanged;
    }

    private void LogPasswordChange(EventId eventId, string result, Guid playerId)
    {
        Dictionary<string, object> properties = new()
        {
            [EventCategoryProperty] = SecurityCategory
        };

        using (IDisposable? scope = _logger.BeginScope(properties))
        {
            _logger.LogInformation(eventId, "Password change {Result}. PlayerId: {PlayerId}", result, playerId);
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
