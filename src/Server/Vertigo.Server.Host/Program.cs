using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess;
using CoreWCF;
using CoreWCF.Configuration;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Implementations;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.Matchmaking.Invitations.Dependencies;
using Vertigo.Server.Matchmaking.Invitations.Services;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Friendships.Dependencies;
using Vertigo.Server.PlayerManagement.Friendships.Services;
using Vertigo.Server.PlayerManagement.Registration.Services;
using Vertigo.Server.PlayerManagement.Blocking.Services;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.PlayerManagement.Registration.Dependencies;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.UseNetTcp(IPAddress.Loopback, ServiceEndpoints.TcpPort);
builder.Services.AddServiceModelServices();
builder.Services.AddVertigoDataAccess(builder.Configuration);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

builder.Services.AddSingleton<IGuestSessionRegistry, GuestSessionRegistry>();
builder.Services.AddScoped<ISessionIdentityResolver, SessionIdentityResolver>();
builder.Services.AddTransient<AccountServiceDependencies>();
builder.Services.AddTransient<AuthenticationDependencies>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<AuthenticationService>();
builder.Services.AddTransient<AccountService>();
builder.Services.AddTransient<BlockedPlayerService>();
builder.Services.AddSingleton<IFriendshipNotifier, FriendshipNotifier>();
builder.Services.AddTransient<FriendshipServiceDependencies>();
builder.Services.AddTransient<FriendshipService>();
builder.Services.AddSingleton<FriendshipNotificationService>();
builder.Services.AddSingleton<IMatchSessionStarter, InMemoryMatchSessionStarter>();
builder.Services.AddSingleton<LobbyMatchActivation>();
builder.Services.AddSingleton<LobbyService>();
builder.Services.AddSingleton<ILobbyDirectory>(services => services.GetRequiredService<LobbyService>());
builder.Services.AddTransient<LobbyInvitationServiceDependencies>();
builder.Services.AddTransient<LobbyInvitationService>();
builder.Services.AddTransient<ChatService>();

WebApplication app = builder.Build();
((IApplicationBuilder)app).UseServiceModel(ConfigureServices);
app.Run();

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
