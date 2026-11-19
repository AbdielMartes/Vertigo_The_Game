using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;

namespace Vertigo.Server.Game.Matches.DependencyInjection;

public static class MatchLifecycleServiceCollectionExtensions
{
    public static IServiceCollection AddMatchLifecycleServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<InMemoryMatchSessionStarter>();
        services.AddSingleton<IMatchSessionStarter>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());
        services.AddSingleton<IMatchLifecycle>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());
        services.AddSingleton<IMatchPresence>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        services.AddSingleton<IMatchBoardReader>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        services.AddSingleton<IMatchPrivateHandReader>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        services.AddSingleton<IMatchPlacementService>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        services.AddSingleton<IMatchDismantleService>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        services.AddSingleton<IMatchTurnClock>(provider =>
            provider.GetRequiredService<InMemoryMatchSessionStarter>());

        return services;
    }
}
