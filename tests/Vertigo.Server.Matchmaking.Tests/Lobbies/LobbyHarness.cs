using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.DataAccess.Repositories.Implementations;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.Game.Matches.DependencyInjection;
using Vertigo.Server.Matchmaking.Lobbies.Match;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Dependencies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Friendships.Services;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

internal sealed class LobbyHarness : IAsyncDisposable
{
    private readonly WebApplication _host;
    private readonly DuplexChannelFactory<ILobbyService> _client;
    private readonly SqliteConnection? _connection;

    private LobbyHarness(WebApplication host, SqliteConnection? connection, EndpointAddress address)
    {
        _host = host;
        _connection = connection;
        Address = address;
        _client = new DuplexChannelFactory<ILobbyService>(new InstanceContext(new LobbyCallbacks()), Binding, address);
        Channel = _client.CreateChannel();
    }

    public EndpointAddress Address { get; }
    public NetTcpBinding Binding { get; } = new(SecurityMode.None) { SendTimeout = TimeSpan.FromSeconds(10) };
    public ILobbyService Channel { get; }
    public IServiceProvider Services => _host.Services;
    public RecordingLogger Log => Services.GetRequiredService<RecordingLogger>();
    public IGuestSessionRegistry Guests => Services.GetRequiredService<IGuestSessionRegistry>();

    public static async Task<LobbyHarness> StartAsync(SqliteConnection? retained = null,
        RecoveryDirectory? recovery = null, TimeProvider? clock = null)
    {
        int port = FindAvailablePort();
        SqliteConnection connection = retained ?? new("Data Source=:memory:");
        if (retained is null)
        {
            connection.Open();
        }
        WebApplicationBuilder builder = WebApplication.CreateSlimBuilder();
        builder.Logging.ClearProviders();
        builder.Services.AddSingleton<RecordingLogger>();
        builder.Services.AddSingleton<ILoggerProvider>(services => services.GetRequiredService<RecordingLogger>());
        builder.WebHost.UseNetTcp(IPAddress.Loopback, port);
        RegisterServices(builder.Services, connection);
        builder.Services.AddSingleton<TimeProvider>(clock ?? TimeProvider.System);
        if (recovery is not null)
        {
            builder.Services.AddSingleton(recovery);
        }
        WebApplication host = builder.Build();
        try
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>().Database.EnsureCreated();
            }
            ((IApplicationBuilder)host).UseServiceModel(services =>
            {
                services.AddService<LobbyService>().AddServiceEndpoint<LobbyService, ILobbyService>(
                    new CoreWCF.NetTcpBinding(CoreWCF.SecurityMode.None), "/Lobby");
                services.ConfigureServiceHostBase<LobbyService>(service => service.Description.Behaviors.Add(
                    new LobbyDispatchErrors(host.Services.GetRequiredService<ILogger<LobbyDispatchErrors>>())));
            });
            await host.StartAsync();
            EndpointAddress address = new($"net.tcp://127.0.0.1:{port}/Lobby");
            return new LobbyHarness(host, retained is null ? connection : null, address);
        }
        catch
        {
            await host.DisposeAsync();
            if (retained is null)
            {
                await connection.DisposeAsync();
            }
            throw;
        }
    }

    private static void RegisterServices(IServiceCollection services, SqliteConnection connection)
    {
        services.AddServiceModelServices();
        services.AddDbContext<VertigoDatabaseContext>(options => options.UseSqlite(connection));
        services.AddScoped<ISessionTokenRepository, SessionTokenRepository>();
        services.AddSingleton<ITokenGenerator, TokenGenerator>();
        services.AddSingleton<IGuestSessionRegistry, GuestSessionRegistry>();
        services.AddScoped<SessionIdentityResolverDependencies>();
        services.AddScoped<ISessionIdentityResolver, SessionIdentityResolver>();
        services.AddSingleton<IFriendshipNotifier, FriendshipNotifier>();
        services.AddMatchLifecycleServices();
        services.AddSingleton<LobbyMatchActivation>();
        services.AddSingleton<LobbyGameOperations>();
        services.AddSingleton<LobbyLifecycle>();
        services.AddSingleton<LobbyService>();
    }

    public string SeedRegisteredSession(DateTime expiration, bool revoked)
    {
        using IServiceScope scope = Services.CreateScope();
        VertigoDatabaseContext database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        ITokenGenerator generator = scope.ServiceProvider.GetRequiredService<ITokenGenerator>();
        string token = generator.GenerateSessionToken();
        Guid playerId = Guid.NewGuid();
        Player player = new()
        {
            PlayerId = playerId,
            Username = playerId.ToString("N")[..16],
            Email = $"{playerId:N}@example.test",
            PasswordHash = "test-hash",
            PasswordSalt = "test-salt",
            CreatedAt = DateTime.UtcNow
        };
        database.Players.Add(player);
        database.SessionTokens.Add(new SessionToken
        {
            TokenId = Guid.NewGuid(),
            PlayerId = playerId,
            TokenValue = generator.HashSessionToken(token),
            ExpiresAt = expiration,
            IsRevoked = revoked,
            CreatedAt = DateTime.UtcNow
        });
        database.SaveChanges();

        return token;
    }

    public async ValueTask DisposeAsync()
    {
        ((IClientChannel)Channel).Abort();
        _client.Abort();
        await _host.StopAsync();
        await _host.DisposeAsync();
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }

    private static int FindAvailablePort()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private sealed class LobbyCallbacks : ILobbyCallback
    {
        public void Notify(LobbyNotification notification)
        {
        }
    }
}

internal sealed class RecordingLogger : ILoggerProvider
{
    private readonly ConcurrentQueue<(int Id, string Message)> _entries = new();

    public IEnumerable<string> Messages(int id)
    {
        return _entries.Where(entry => entry.Id == id).Select(entry => entry.Message);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new Capture(_entries);
    }

    public void Dispose()
    {
    }

    private sealed class Capture(ConcurrentQueue<(int Id, string Message)> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= LogLevel.Warning;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                entries.Enqueue((eventId.Id, formatter(state, exception)));
            }
        }
    }
}
