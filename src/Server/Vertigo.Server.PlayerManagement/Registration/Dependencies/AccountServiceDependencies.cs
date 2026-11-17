using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Registration.Dependencies;

public sealed record AccountServiceDependencies(
    IPlayerRepository PlayerRepository,
    ISessionTokenRepository SessionTokenRepository,
    IPasswordHasher PasswordHasher,
    ISessionIdentityResolver Sessions);
