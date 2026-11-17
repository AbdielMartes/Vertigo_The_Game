using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.PlayerManagement.Authentication.Services;

public sealed class SessionIdentityResolver : ISessionIdentityResolver
{
    private const int SessionTokenLength = 64;

    private readonly ISessionTokenRepository _sessionTokenRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IGuestSessionRegistry _guests;

    public SessionIdentityResolver(
        ISessionTokenRepository sessionTokenRepository,
        ITokenGenerator tokenGenerator,
        IGuestSessionRegistry guests)
    {
        _sessionTokenRepository = sessionTokenRepository;
        _tokenGenerator = tokenGenerator;
        _guests = guests;
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
        return Resolve(token) ?? throw CreateUnauthorizedFault();
    }

    public SessionIdentity RequireRegistered(string token)
    {
        SessionIdentity identity = Require(token);
        if (identity.IsGuest)
        {
            throw CreateUnauthorizedFault();
        }

        return identity;
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
