using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface IMatchPrivateHandReader
{
    MatchPrivateHand? GetPrivateHand(Guid matchId, Guid playerId);
}
