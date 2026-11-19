using CoreWCF;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.DataAccess.Repositories.Interfaces;

namespace Vertigo.Server.PlayerManagement.Blocking.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single)]
public class BlockedPlayerService : IBlockedPlayerService
{
    private readonly IBlockedPlayerRepository _blockedPlayerRepository;
    private readonly ISessionIdentityResolver _sessions;

    public BlockedPlayerService(IBlockedPlayerRepository blockedPlayerRepository, ISessionIdentityResolver sessions)
    {
        _blockedPlayerRepository = blockedPlayerRepository;
        _sessions = sessions;
    }

    public List<BlockedPlayerSummary> GetBlockedPlayers(string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return GetBlockedPlayersForPlayer(identity.PlayerId);
    }

    public bool BlockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return BlockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    public bool UnblockPlayer(Guid otherPlayerId, string sessionToken)
    {
        SessionIdentity identity = _sessions.RequireRegistered(sessionToken);
        return UnblockPlayerBetween(new PlayerPair { PlayerId = identity.PlayerId, OtherPlayerId = otherPlayerId });
    }

    private List<BlockedPlayerSummary> GetBlockedPlayersForPlayer(Guid playerId)
    {
        if (playerId == Guid.Empty)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        List<BlockedPlayer> blockedPlayers = _blockedPlayerRepository.GetBlockedPlayers(playerId);
        return blockedPlayers.Select(blocked => new BlockedPlayerSummary
        {
            PlayerId = blocked.PlayerBlockedId,
            Username = blocked.PlayerBlocked.Username,
            BlockedAt = blocked.CreatedAt
        }).ToList();
    }

    private bool BlockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        if (_blockedPlayerRepository.IsPlayerBlocked(pair.PlayerId, pair.OtherPlayerId))
        {
            return false;
        }

        return _blockedPlayerRepository.BlockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private bool UnblockPlayerBetween(PlayerPair pair)
    {
        EnsureValidPair(pair);
        return _blockedPlayerRepository.UnblockPlayer(pair.PlayerId, pair.OtherPlayerId);
    }

    private static void EnsureValidPair(PlayerPair pair)
    {
        if (pair is null || pair.PlayerId == Guid.Empty || pair.OtherPlayerId == Guid.Empty
            || pair.PlayerId == pair.OtherPlayerId)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new ServiceFault { ErrorCode = errorCode, MessageKey = messageKey };
        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
