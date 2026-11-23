using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Repositories.Implementations;
using Vertigo.Server.DataAccess.Repositories.Interfaces;
using Vertigo.Server.DataAccess.Repositories.Statistics;

namespace Vertigo.Server.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddVertigoDataAccess(
        this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("VertigoDb")
            ?? throw new InvalidOperationException("Connection string 'VertigoDb' is missing.");

        services.AddDbContext<VertigoDatabaseContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IBlockedPlayerRepository, BlockedPlayerRepository>();
        services.AddScoped<ISessionTokenRepository, SessionTokenRepository>();
        services.AddScoped<MatchStatisticsRepository>();

        return services;
    }
}
