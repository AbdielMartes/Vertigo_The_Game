using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Registration;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class AccountService : IAccountService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AccountPasswordService _passwordService;

    public AccountService(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        AccountPasswordService passwordService)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _passwordService = passwordService;
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

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
