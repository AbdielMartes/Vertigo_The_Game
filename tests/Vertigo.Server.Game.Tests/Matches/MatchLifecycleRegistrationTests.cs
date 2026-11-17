using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.DependencyInjection;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchLifecycleRegistrationTests
{
    [Fact]
    public void AddMatchLifecycleServices_ResolvedInterfaces_ShareGameRegistry()
    {
        ServiceCollection services = new();
        services.AddSingleton(TimeProvider.System);
        services.AddMatchLifecycleServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        IMatchSessionStarter starter = provider.GetRequiredService<IMatchSessionStarter>();
        IMatchLifecycle lifecycle = provider.GetRequiredService<IMatchLifecycle>();
        Guid host = Guid.NewGuid();
        MatchStartRequest request = new()
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = host,
            PlayerIds = new[] { host, Guid.NewGuid() }
        };

        MatchSessionSnapshot match = starter.Start(request);
        bool finished = lifecycle.TryFinish(match.MatchId);

        Assert.True(finished);
        Assert.Equal(MatchLifecycleStatus.Finished, lifecycle.Get(match.MatchId)?.Status);
        Assert.Equal(2, lifecycle.Get(match.MatchId)?.Revision);
        Assert.Same(starter, lifecycle);
    }
}
