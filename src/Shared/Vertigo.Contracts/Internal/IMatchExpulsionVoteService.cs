namespace Vertigo.Contracts.Internal;

public interface IMatchExpulsionVoteService
{
    MatchExpulsionVoteResult TryVoteToExpel(Guid matchId, Guid voterId, MatchExpulsionVote vote);
}
