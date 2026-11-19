namespace Vertigo.Contracts.Internal;

public enum MatchExpulsionVoteResult
{
    Recorded,
    Expelled,
    MatchUnavailable,
    TurnExpired,
    StaleVotingRevision,
    InvalidVoter,
    InvalidTarget,
    TooFewPlayers,
    AlreadyVoted
}
