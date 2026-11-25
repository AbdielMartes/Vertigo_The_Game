using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Registration;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class AccountService : IAccountService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AccountPasswordService _passwordService;
    private readonly ISessionIdentityResolver _sessions;

    public AccountService(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        AccountPasswordService passwordService,
        ISessionIdentityResolver sessions)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _passwordService = passwordService;
        _sessions = sessions;
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
        return _passwordService.ChangePassword(change, sessionToken);
    }

    public bool UpdateProfile(ProfileUpdate profile, string sessionToken)
    {
        Guid playerId = _sessions.RequireRegistered(sessionToken).PlayerId;
        if (profile is null || RegistrationRules.ValidateUsername(profile.Username) != FieldValidationError.None)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        string? country = CountryCatalog.Normalize(profile.CountryCode);
        bool hasInvalidCountry = !string.IsNullOrWhiteSpace(profile.CountryCode) && country is null;
        bool hasInvalidAvatar = profile.AvatarId is int avatar && !Enum.IsDefined((ProfileAvatar)avatar);
        if (hasInvalidCountry || hasInvalidAvatar)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        PlayerProfileUpdateResult result = _playerRepository.UpdateProfile(
            playerId, profile.Username.Trim(), country, profile.AvatarId);
        if (result == PlayerProfileUpdateResult.UsernameTaken)
        {
            throw CreateFault(ServiceErrorCodes.UsernameAlreadyRegistered, ServiceMessageKeys.UsernameAlreadyRegistered);
        }

        if (result == PlayerProfileUpdateResult.PlayerNotFound)
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        return result == PlayerProfileUpdateResult.Updated;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
