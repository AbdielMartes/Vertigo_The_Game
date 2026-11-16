using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Dependencies;

public sealed class AuthenticationDependencies
{
    public IPlayerRepository PlayerRepository { get; }

    public ISessionTokenRepository SessionTokenRepository { get; }

    public IPasswordHasher PasswordHasher { get; }

    public ITokenGenerator TokenGenerator { get; }

    public AuthenticationDependencies(
        IPlayerRepository playerRepository,
        ISessionTokenRepository sessionTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        PlayerRepository = playerRepository;
        SessionTokenRepository = sessionTokenRepository;
        PasswordHasher = passwordHasher;
        TokenGenerator = tokenGenerator;
    }
}
