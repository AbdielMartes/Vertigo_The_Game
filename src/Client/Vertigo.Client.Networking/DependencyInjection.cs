using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Proxies;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.Networking.Sessions;
using Vertigo.Client.Networking.Sessions.Services;

namespace Vertigo.Client.Networking;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoClientNetworking(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<PlayerSession>();
        services.AddSingleton<INavigator, Navigator>();
        services.AddSingleton<IServiceCaller, ServiceCaller>();
        services.AddTransient<IAuthClient, AuthClient>();
        services.AddTransient<IAccountClient, AccountClient>();
        services.AddTransient<IBlockedPlayerClient, BlockedPlayerClient>();
        services.AddTransient<IFriendshipClient, FriendshipClient>();
        services.AddTransient<ILobbyInvitationClient, LobbyInvitationClient>();
        services.AddTransient<IChatClient, ChatClient>();
        services.AddTransient<IStatisticsClient, StatisticsClient>();
        services.AddSingleton<ISessionCoordinator, SessionCoordinator>();
        services.AddSingleton<ILobbyClient, LobbyClient>();
        services.AddSingleton<IFriendshipNotificationClient, FriendshipNotificationClient>();
        services.AddSingleton<ISessionTransportCleaner, SessionTransportCleaner>();
        return services;
    }
}
