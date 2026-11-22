namespace Vertigo.Contracts.Internal;

public interface IMatchRecovery
{
    IReadOnlyList<(MatchSessionSnapshot Session, RecoverableLobby Lobby, IReadOnlyList<Guid> ActivePlayers)> GetRecoveredMatches();
}
