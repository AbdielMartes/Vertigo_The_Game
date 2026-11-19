using CoreWCF;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

internal static class LobbyPlacementFault
{
    public static FaultException<ServiceFault> Create(MatchPlacementResult result)
    {
        (string errorCode, string messageKey) = result switch
        {
            MatchPlacementResult.NotYourTurn => ("MATCH_NOT_YOUR_TURN", "Match.Error.NotYourTurn"),
            MatchPlacementResult.CardNotOwned => ("MATCH_CARD_NOT_OWNED", "Match.Error.CardNotOwned"),
            MatchPlacementResult.InvalidPile => ("MATCH_INVALID_PILE", "Match.Error.InvalidPile"),
            MatchPlacementResult.InvalidPlacement => ("MATCH_UNSTABLE_PLACEMENT", "Match.Error.Unstable"),
            MatchPlacementResult.StaleRevision => ("MATCH_STALE_REVISION", "Match.Error.Stale"),
            _ => ("MATCH_UNAVAILABLE", "Match.Error.Unavailable")
        };
        ServiceFault fault = new()
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
