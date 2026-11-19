using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.DependencyInjection;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchLifecycleRegistrationTests
{
    [Fact]
    public void TryLeaveMatch_ThroughCommandContract_UpdatesBoardAndHandThroughReader()
    {
        ServiceCollection services = new();
        services.AddSingleton(TimeProvider.System);
        services.AddMatchLifecycleServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        IMatchSessionStarter starter = provider.GetRequiredService<IMatchSessionStarter>();
        IMatchStateReader reader = provider.GetRequiredService<IMatchStateReader>();
        IMatchCommandService commands = provider.GetRequiredService<IMatchCommandService>();
        Guid leaving = Guid.NewGuid();
        Guid remaining = Guid.NewGuid();
        MatchSessionSnapshot match = starter.Start(new MatchStartRequest
        {
            LobbyInstanceId = Guid.NewGuid(),
            LobbyCode = "ABCDEF",
            HostPlayerId = leaving,
            PlayerIds = new[] { leaving, remaining }
        });
        Assert.NotNull(reader.GetPrivateHand(match.MatchId, leaving));

        MatchDepartureResult result = commands.TryLeaveMatch(match.MatchId, leaving);

        Assert.Equal(new MatchDepartureResult(true, true), result);
        Assert.Null(reader.GetPrivateHand(match.MatchId, leaving));
        Assert.Equal(remaining, Assert.Single(reader.GetBoard(match.MatchId)!.WinnerIds));
    }

    [Fact]
    public void AddMatchLifecycleServices_ResolvedInterfaces_ShareGameRegistry()
    {
        ServiceCollection services = new();
        services.AddSingleton(TimeProvider.System);
        services.AddMatchLifecycleServices();
        using ServiceProvider provider = services.BuildServiceProvider();
        IMatchSessionStarter starter = provider.GetRequiredService<IMatchSessionStarter>();
        IMatchLifecycle lifecycle = provider.GetRequiredService<IMatchLifecycle>();
        IMatchStateReader stateReader = provider.GetRequiredService<IMatchStateReader>();
        IMatchCommandService commands = provider.GetRequiredService<IMatchCommandService>();
        IMatchDismantleService dismantle = provider.GetRequiredService<IMatchDismantleService>();
        IMatchTurnClock clock = provider.GetRequiredService<IMatchTurnClock>();
        IMatchExpulsionVoteService votes = provider.GetRequiredService<IMatchExpulsionVoteService>();
        IMatchDepartureService departures = provider.GetRequiredService<IMatchDepartureService>();
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
        Assert.Same(starter, stateReader);
        Assert.Same(starter, commands);
        Assert.Equal(match.MatchId, stateReader.GetBoard(match.MatchId)?.MatchId);
        Assert.Same(starter, dismantle);
        Assert.Same(starter, clock);
        Assert.Same(starter, votes);
        Assert.Same(starter, departures);
        Assert.Null(clock.TryExpireTurn(match.MatchId));
        Assert.Equal(MatchDismantleResult.MatchUnavailable,
            commands.TryDismantle(match.MatchId, host, new MatchDismantle(1, 1)));
    }
}
