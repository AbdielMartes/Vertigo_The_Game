using System.Net;
using CoreWCF.Configuration;
using CoreWCF;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess;
using Vertigo.Server.Game.Matches.DependencyInjection;
using Vertigo.Server.Host.Logging;
using Vertigo.Server.Host.Matches;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.Matchmaking.Invitations.Services;
using Vertigo.Server.Matchmaking.Lobbies.Match;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Blocking.Services;
using Vertigo.Server.PlayerManagement.Friendships.Services;
using Vertigo.Server.PlayerManagement.Registration.Services;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.Logging.ClearProviders();
Serilog.Core.Logger logger = ServerLoggingConfiguration.CreateLogger(builder.Configuration, builder.Environment);
builder.Services.AddSerilog(logger, dispose: false);

try
{
    builder.WebHost.UseNetTcp(IPAddress.Loopback, ServiceEndpoints.TcpPort);
    builder.Services.AddServiceModelServices();
    builder.Services.AddVertigoDataAccess(builder.Configuration);

    builder.Services.AddSingleton(TimeProvider.System);
    builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
    builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

    builder.Services.AddSingleton<IGuestSessionRegistry, GuestSessionRegistry>();
    builder.Services.AddScoped<SessionIdentityResolverDependencies>();
    builder.Services.AddScoped<ISessionIdentityResolver, SessionIdentityResolver>();
    builder.Services.AddTransient<AccountPasswordStore>();
    builder.Services.AddTransient<AccountPasswordService>();
    builder.Services.AddTransient<PlayerCredentialsVerifier>();
    builder.Services.AddTransient<RegisteredPlayerSessions>();
    builder.Services.AddTransient<AuthService>();
    builder.Services.AddTransient<AuthenticationService>();
    builder.Services.AddTransient<AccountService>();
    builder.Services.AddTransient<BlockedPlayerService>();
    builder.Services.AddSingleton<IFriendshipNotifier, FriendshipNotifier>();
    builder.Services.AddTransient<FriendshipQueries>();
    builder.Services.AddTransient<FriendshipCommands>();
    builder.Services.AddTransient<FriendshipService>();
    builder.Services.AddSingleton<FriendshipNotificationService>();
    builder.Services.AddMatchLifecycleServices();
    builder.Services.AddSingleton<LobbyMatchActivation>();
    builder.Services.AddSingleton<LobbyGameOperations>();
    builder.Services.AddSingleton<LobbyLifecycle>();
    builder.Services.AddSingleton<LobbyService>();
    builder.Services.AddHostedService<MatchTurnClockWorker>();
    builder.Services.AddSingleton<ILobbyDirectory>(services => services.GetRequiredService<LobbyService>());
    builder.Services.AddTransient<LobbyInvitationFriendLookup>();
    builder.Services.AddTransient<LobbyInvitationOperations>();
    builder.Services.AddTransient<LobbyInvitationService>();
    // A nonempty ChatModeration:BlockedTerms list replaces the built-in dictionary.
    IEnumerable<string> blockedChatTerms = builder.Configuration.GetSection("ChatModeration:BlockedTerms")
        .GetChildren().Select(entry => entry.Value).OfType<string>();
    builder.Services.AddSingleton(new ChatLanguageFilter(blockedChatTerms));
    builder.Services.AddScoped<ChatServiceDependencies>();
    builder.Services.AddTransient<ChatService>();

    WebApplication app = builder.Build();
    ((IApplicationBuilder)app).UseServiceModel(ConfigureServices);
    app.Run();
}
finally
{
    try
    {
        logger.Dispose();
    }
    // At the Host shutdown boundary, sink disposal must not mask a startup or runtime failure.
    catch (Exception)
    {
        Console.Error.WriteLine("Logging shutdown failed; some events may be missing.");
    }
}

static void ConfigureServices(IServiceBuilder services)
{
    NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
    services.AddService<AccountService>()
        .AddServiceEndpoint<AccountService, IAccountService>(binding, ServiceEndpoints.AccountServiceAddress);
    services.AddService<AuthenticationService>()
        .AddServiceEndpoint<AuthenticationService, IAuthService>(binding, ServiceEndpoints.AuthServiceAddress);
    services.AddService<BlockedPlayerService>()
        .AddServiceEndpoint<BlockedPlayerService, IBlockedPlayerService>(
            binding, ServiceEndpoints.BlockedPlayerServiceAddress);
    services.AddService<FriendshipService>()
        .AddServiceEndpoint<FriendshipService, IFriendshipService>(binding, ServiceEndpoints.FriendshipServiceAddress);
    services.AddService<FriendshipNotificationService>()
        .AddServiceEndpoint<FriendshipNotificationService, IFriendshipNotificationService>(
            binding, ServiceEndpoints.FriendshipNotificationServiceAddress);
    services.AddService<ChatService>()
        .AddServiceEndpoint<ChatService, IChatService>(binding, ServiceEndpoints.ChatServiceAddress);
    services.AddService<LobbyService>()
        .AddServiceEndpoint<LobbyService, ILobbyService>(binding, ServiceEndpoints.LobbyServiceAddress);
    services.AddService<LobbyInvitationService>()
        .AddServiceEndpoint<LobbyInvitationService, ILobbyInvitationService>(
            binding, ServiceEndpoints.LobbyInvitationServiceAddress);
}
