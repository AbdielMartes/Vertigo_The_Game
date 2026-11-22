using Vertigo.Contracts.Models;

namespace Vertigo.Contracts.Synchronization;

public sealed record MatchSyncState(MatchBoardSnapshot Board, MatchPrivateHand? Hand);
