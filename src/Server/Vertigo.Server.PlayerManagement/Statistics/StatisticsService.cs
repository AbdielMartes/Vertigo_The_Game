using CoreWCF;
using Vertigo.Registration;
using Vertigo.Contracts.Faults;
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
        Guid viewerId = sessions.Require(sessionToken).PlayerId;
        return MapRanking(repository.GetRanking(RankingLimit, viewerId));
    }

    public List<RankingEntry> GetFriendsRanking(string sessionToken)
    {
        Guid viewerId = sessions.RequireRegistered(sessionToken).PlayerId;
        return MapRanking(repository.GetFriendsRanking(viewerId));
    }

    public List<RankingEntry> GetCountryRanking(string sessionToken)
    {
        Guid viewerId = sessions.RequireRegistered(sessionToken).PlayerId;
        if (CountryCatalog.Normalize(repository.GetPlayerProfile(viewerId)?.CountryCode) is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.RankingCountryRequired);
        }

        return MapRanking(repository.GetCountryRanking(viewerId, RankingLimit));
    }

    public PlayerMatchRecord GetMyRecord(string sessionToken)
    {
        Guid playerId = sessions.RequireRegistered(sessionToken).PlayerId;
        PlayerProfileRecord? profile = repository.GetPlayerProfile(playerId);
        if (profile is null)
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        return MapRecord(profile);
    }

    public PublicPlayerProfile GetPlayerProfile(Guid playerId, string sessionToken)
    {
        Guid viewerId = sessions.Require(sessionToken).PlayerId;
        PlayerProfileRecord? profile = repository.GetPlayerProfile(playerId, viewerId);
        if (profile is null)
        {
            throw CreateFault(ServiceErrorCodes.UserNotFound, ServiceMessageKeys.UserNotFound);
        }

        return new PublicPlayerProfile { Username = profile.Username, Record = MapRecord(profile) };
    }

    private static List<RankingEntry> MapRanking(IEnumerable<MatchStatisticsRepository.RankingPlayer> rows)
    {
        return rows.Select(row => new RankingEntry
        {
            PlayerId = row.PlayerId,
            AvatarId = row.AvatarId,
            Username = row.Username,
            MatchesWon = row.MatchesWon,
            FloorsBuilt = row.FloorsBuilt
        }).ToList();
    }

    private static PlayerMatchRecord MapRecord(PlayerProfileRecord profile)
    {
        return new PlayerMatchRecord
        {
            MatchesPlayed = profile.MatchesPlayed,
            MatchesWon = profile.MatchesWon,
            CreatedYear = profile.CreatedYear,
            AvatarId = profile.AvatarId,
            CountryCode = profile.CountryCode
        };
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new() { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
