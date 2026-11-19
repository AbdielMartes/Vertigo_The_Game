using CoreWCF;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Registration;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public class AccountService : IAccountService
{
    private const string EventCategoryProperty = "EventCategory";
    private const string SecurityCategory = "Security";
    private const string SucceededResult = "Succeeded";
    private const string RejectedResult = "Rejected";

    private static readonly EventId PasswordChangeSucceededEvent = new(2101, "PasswordChangeSucceeded");
    private static readonly EventId PasswordChangeRejectedEvent = new(2102, "PasswordChangeRejected");

    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionIdentityResolver _sessions;
    private readonly ILogger<AccountService> _logger;

    public AccountService(AccountServiceDependencies dependencies, ILogger<AccountService> logger)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _sessions = dependencies.Sessions;
        _logger = logger;
    }

    public bool RegisterAccount(AccountRegistration registration)
    {
        if (registration is null
            || RegistrationRules.ValidateUsername(registration.Username) != FieldValidationError.None
            || RegistrationRules.ValidateEmail(registration.Email) != FieldValidationError.None
            || RegistrationRules.ValidatePassword(registration.Password) != FieldValidationError.None)
        {
            throw CreateFault(ServiceErrorCodes.InvalidRegistrationData, ServiceMessageKeys.InvalidRegistrationData);
        }

        string username = registration.Username.Trim();
        string email = registration.Email.Trim();

        if (_playerRepository.UsernameExists(username))
        {
            throw CreateFault(ServiceErrorCodes.UsernameAlreadyRegistered,
                ServiceMessageKeys.UsernameAlreadyRegistered);
        }

        if (_playerRepository.EmailExists(email))
        {
            throw CreateFault(ServiceErrorCodes.EmailAlreadyRegistered,
                ServiceMessageKeys.EmailAlreadyRegistered);
        }

        string salt = _passwordHasher.GenerateSalt();
        Player player = new Player
        {
            Username = username,
            Email = email,
            PasswordSalt = salt,
            PasswordHash = _passwordHasher.HashPassword(registration.Password, salt),
            Stats = new PlayerStats()
        };

        return _playerRepository.CreatePlayer(player);
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
        Player? player = _playerRepository.GetPlayerById(identity.PlayerId);
        if (player is null
            || !string.Equals(player.Username, change.Username, StringComparison.OrdinalIgnoreCase)
            || !_passwordHasher.VerifyPassword(change.CurrentPassword, player.PasswordHash, player.PasswordSalt))
        {
            LogPasswordChange(PasswordChangeRejectedEvent, RejectedResult, identity.PlayerId);
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        string salt = _passwordHasher.GenerateSalt();
        string hash = _passwordHasher.HashPassword(change.NewPassword, salt);
        bool isChanged = _playerRepository.UpdatePassword(player.PlayerId, hash, salt);
        if (isChanged)
        {
            _sessionTokenRepository.RevokePlayerSessions(player.PlayerId);
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
