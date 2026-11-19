using CoreWCF;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

internal static class LobbyMatchFault
{
    public static FaultException<ServiceFault> Create(MatchPlacementResult result)
    {
        (string errorCode, string messageKey) = result switch
        {
            MatchPlacementResult.NotYourTurn => ("MATCH_NOT_YOUR_TURN", "Match.Error.NotYourTurn"),
            MatchPlacementResult.CardNotOwned => ("MATCH_CARD_NOT_OWNED", "Match.Error.CardNotOwned"),
            MatchPlacementResult.InvalidPile => ("MATCH_INVALID_PILE", "Match.Error.InvalidPile"),
            MatchPlacementResult.InvalidPlacement => ("MATCH_UNSTABLE_PLACEMENT", "Match.Error.Unstable"),
            MatchPlacementResult.MasterPlanRequired => ("MATCH_MASTER_PLAN_REQUIRED", "Match.Error.MasterPlanRequired"),
            MatchPlacementResult.InvalidMasterPlanChain =>
                ("MATCH_INVALID_MASTER_PLAN_CHAIN", "Match.Error.InvalidMasterPlanChain"),
            MatchPlacementResult.TurnExpired => ("MATCH_TURN_EXPIRED", "Match.Error.TurnExpired"),
            MatchPlacementResult.StaleRevision => ("MATCH_STALE_REVISION", "Match.Error.Stale"),
            _ => ("MATCH_UNAVAILABLE", "Match.Error.Unavailable")
        };
        return Create(errorCode, messageKey);
    }

    public static FaultException<ServiceFault> Create(MatchDismantleResult result)
    {
        FaultException<ServiceFault> fault = result switch
        {
            MatchDismantleResult.NotYourTurn => Create(MatchPlacementResult.NotYourTurn),
            MatchDismantleResult.InvalidPile => Create(MatchPlacementResult.InvalidPile),
            MatchDismantleResult.StaleRevision => Create(MatchPlacementResult.StaleRevision),
            MatchDismantleResult.TurnExpired => Create(MatchPlacementResult.TurnExpired),
            MatchDismantleResult.EmptyPile => Create("MATCH_EMPTY_TOWER", "Match.Error.EmptyTower"),
            MatchDismantleResult.PlacementAvailable =>
                Create("MATCH_PLACEMENT_AVAILABLE", "Match.Error.PlacementAvailable"),
            _ => Create(MatchPlacementResult.MatchUnavailable)
        };

        return fault;
    }

    public static FaultException<ServiceFault> Create(MatchExpulsionVoteResult result)
    {
        (string code, string key) = result switch
        {
            MatchExpulsionVoteResult.InvalidVoter => ("MATCH_INVALID_VOTER", "Match.Error.InvalidVoter"),
            MatchExpulsionVoteResult.InvalidTarget => ("MATCH_INVALID_VOTE_TARGET", "Match.Error.InvalidVoteTarget"),
            MatchExpulsionVoteResult.TooFewPlayers => ("MATCH_VOTE_TOO_FEW_PLAYERS", "Match.Error.TooFewPlayersToVote"),
            MatchExpulsionVoteResult.AlreadyVoted => ("MATCH_ALREADY_VOTED", "Match.Error.AlreadyVoted"),
            MatchExpulsionVoteResult.StaleVotingRevision => ("MATCH_STALE_VOTE", "Match.Error.StaleVote"),
            MatchExpulsionVoteResult.TurnExpired => ("MATCH_TURN_EXPIRED", "Match.Error.TurnExpired"),
            _ => ("MATCH_UNAVAILABLE", "Match.Error.Unavailable")
        };

        return Create(code, key);
    }

    private static FaultException<ServiceFault> Create(string errorCode, string messageKey)
    {
        ServiceFault fault = new()
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
