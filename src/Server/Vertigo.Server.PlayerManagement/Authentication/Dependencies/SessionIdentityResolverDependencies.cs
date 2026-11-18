using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Authentication.Dependencies;

public sealed class SessionIdentityResolverDependencies
{
    public ISessionTokenRepository SessionTokenRepository { get; }

    public ITokenGenerator TokenGenerator { get; }

    public IGuestSessionRegistry Guests { get; }

    public SessionIdentityResolverDependencies(
        ISessionTokenRepository sessionTokenRepository,
        ITokenGenerator tokenGenerator,
        IGuestSessionRegistry guests)
    {
        SessionTokenRepository = sessionTokenRepository;
        TokenGenerator = tokenGenerator;
        Guests = guests;
    }
}
