using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Services;

namespace Vertigo.Server.Matchmaking.Tests.Authentication;

public sealed class GuestSessionTests
{
    [Fact]
    public void Find_AtExactExpiration_ReturnsNull()
    {
        ControlledTimeProvider clock = new(new DateTimeOffset(2026, 11, 16, 12, 0, 0, TimeSpan.Zero));
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string hash = tokens.HashSessionToken(registry.Create().Token);
        Assert.NotNull(registry.Find(hash));

        clock.Advance(TimeSpan.FromHours(12));

        Assert.Null(registry.Find(hash));
    }

    [Fact]
    public void Revoke_RemovesGuestSession()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string hash = tokens.HashSessionToken(registry.Create().Token);

        Assert.True(registry.Revoke(hash));
        Assert.Null(registry.Find(hash));
    }

    [Fact]
    public void RequireRegistered_GuestIsRejected()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string raw = registry.Create().Token;
        SessionIdentityResolver resolver = new(new FakeSessionTokenRepository(), tokens, registry);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => resolver.RequireRegistered(raw));

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
    }

    [Fact]
    public void Require_RevokedGuestIsRejected()
    {
        ControlledTimeProvider clock = new(DateTimeOffset.UtcNow);
        FakeTokenGenerator tokens = new();
        GuestSessionRegistry registry = new(tokens, clock);
        string raw = registry.Create().Token;
        registry.Revoke(tokens.HashSessionToken(raw));
        SessionIdentityResolver resolver = new(new FakeSessionTokenRepository(), tokens, registry);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => resolver.Require(raw));

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
    }

    private sealed class ControlledTimeProvider : TimeProvider
    {
        private DateTimeOffset _utcNow;

        public ControlledTimeProvider(DateTimeOffset utcNow)
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }

        public void Advance(TimeSpan amount)
        {
            _utcNow += amount;
        }
    }

    private sealed class FakeTokenGenerator : ITokenGenerator
    {
        public string GenerateSessionToken()
        {
            return new string('A', 64);
        }

        public string GenerateVerificationCode()
        {
            return "123456";
        }

        public string HashSessionToken(string sessionToken)
        {
            return sessionToken;
        }
    }

    private sealed class FakeSessionTokenRepository : ISessionTokenRepository
    {
        public bool CreateSessionToken(SessionToken token)
        {
            return false;
        }

        public SessionToken? GetActiveSessionToken(string tokenHash)
        {
            return null;
        }

        public bool RevokeSessionToken(string tokenHash)
        {
            return false;
        }

        public int RevokePlayerSessions(Guid playerId)
        {
            return 0;
        }
    }
}
