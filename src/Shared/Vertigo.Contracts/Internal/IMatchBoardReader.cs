using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Internal;

public interface IMatchBoardReader
{
    MatchBoardSnapshot? GetBoard(Guid matchId);
}
