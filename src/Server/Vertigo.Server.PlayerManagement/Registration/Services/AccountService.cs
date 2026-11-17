using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

namespace Vertigo.Server.PlayerManagement.Registration.Services;

public class AccountService : IAccountService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISessionIdentityResolver _sessions;

    public AccountService(AccountServiceDependencies dependencies)
    {
        _playerRepository = dependencies.PlayerRepository;
        _sessionTokenRepository = dependencies.SessionTokenRepository;
        _passwordHasher = dependencies.PasswordHasher;
        _sessions = dependencies.Sessions;
    }

    public bool RegisterAccount(AccountRegistration registration)
    {
        if (registration is null || string.IsNullOrWhiteSpace(registration.Username)
            || string.IsNullOrWhiteSpace(registration.Email)
            || string.IsNullOrWhiteSpace(registration.Password)
            || registration.Username.Length > 32 || registration.Email.Length > 120)
        {
            throw CreateFault(ServiceErrorCodes.InvalidRegistrationData, ServiceMessageKeys.InvalidRegistrationData);
        }

        if (_playerRepository.UsernameExists(registration.Username))
        {
            throw CreateFault(ServiceErrorCodes.UsernameAlreadyRegistered,
                ServiceMessageKeys.UsernameAlreadyRegistered);
        }

        if (_playerRepository.EmailExists(registration.Email))
        {
            throw CreateFault(ServiceErrorCodes.EmailAlreadyRegistered,
                ServiceMessageKeys.EmailAlreadyRegistered);
        }

        string salt = _passwordHasher.GenerateSalt();
        Player player = new Player
        {
            Username = registration.Username.Trim(),
            Email = registration.Email.Trim(),
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
        if (player is null || !string.Equals(player.Username, change.Username, StringComparison.OrdinalIgnoreCase) || !_passwordHasher.VerifyPassword(
                change.CurrentPassword, player.PasswordHash, player.PasswordSalt))
        {
            throw CreateFault(ServiceErrorCodes.InvalidCredentials, ServiceMessageKeys.InvalidCredentials);
        }

        string salt = _passwordHasher.GenerateSalt();
        string hash = _passwordHasher.HashPassword(change.NewPassword, salt);
        bool isChanged = _playerRepository.UpdatePassword(player.PlayerId, hash, salt);
        if (isChanged)
        {
            _sessionTokenRepository.RevokePlayerSessions(player.PlayerId);
        }

        return isChanged;
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
