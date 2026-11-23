using CoreWCF;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Repositories.Statistics;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.PlayerManagement.Statistics;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public sealed class StatisticsService(ISessionIdentityResolver sessions, MatchStatisticsRepository repository)
    : IStatisticsService
{
    private const int RankingLimit = 50;

    public List<RankingEntry> GetRanking(string sessionToken)
    {
        sessions.Require(sessionToken);
        return repository.GetRanking(RankingLimit).Select(row => new RankingEntry
        {
            Username = row.Username, MatchesWon = row.MatchesWon, FloorsBuilt = row.FloorsBuilt
        }).ToList();
    }

    public PlayerMatchRecord GetMyRecord(string sessionToken)
    {
        Guid playerId = sessions.RequireRegistered(sessionToken).PlayerId;
        (int played, int won) = repository.GetRecord(playerId);
        return new PlayerMatchRecord { MatchesPlayed = played, MatchesWon = won };
    }
}
