using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Matchmaking.Lobbies.Match;

public sealed record MatchReadServices(IMatchBoardReader Board, IMatchPrivateHandReader Hands,
    IMatchPlacementService Placements);
