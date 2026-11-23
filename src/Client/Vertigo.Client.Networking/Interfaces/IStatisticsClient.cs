using Vertigo.Client.Networking.Results;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.Networking.Interfaces;

public interface IStatisticsClient
{
    ClientResult<List<RankingEntry>> GetRanking(string sessionToken);
    ClientResult<PlayerMatchRecord> GetMyRecord(string sessionToken);
}
