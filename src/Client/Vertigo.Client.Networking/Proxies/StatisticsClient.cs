using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Proxies;

public sealed class StatisticsClient(IServiceCaller serviceCaller) : IStatisticsClient
{
    public ClientResult<List<RankingEntry>> GetRanking(string sessionToken)
    {
        return serviceCaller.Call<IStatisticsService, string, List<RankingEntry>>(
            ServiceEndpoints.StatisticsServiceAddress, (channel, token) => channel.GetRanking(token), sessionToken);
    }

    public ClientResult<PlayerMatchRecord> GetMyRecord(string sessionToken)
    {
        return serviceCaller.Call<IStatisticsService, string, PlayerMatchRecord>(
            ServiceEndpoints.StatisticsServiceAddress, (channel, token) => channel.GetMyRecord(token), sessionToken);
    }
}
