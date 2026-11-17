using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

public sealed record MatchReadServices(IMatchBoardReader Board, IMatchPrivateHandReader Hands,
    IMatchPlacementService Placements);
