using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Models;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Implementations;

// One synchronized in-memory registry for idempotent starts and terminal transitions.
// Deliberately no automatic cancellation when lobby connections disappear (Phase 7B).
public sealed class InMemoryMatchSessionStarter : IMatchSessionStarter, IMatchLifecycle, IMatchPresence,
    IMatchBoardReader, IMatchPrivateHandReader, IMatchPlacementService, IMatchDismantleService
{
    private const int MinimumPlayerCount = 2;
    private const int MaximumPlayerCount = 6;

    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesByLobby = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesById = new();
    private readonly TimeProvider _timeProvider;

    public InMemoryMatchSessionStarter(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public MatchSessionSnapshot Start(MatchStartRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Validate(request);

        MatchSessionSnapshot result;
        lock (_syncRoot)
        {
            if (_matchesByLobby.TryGetValue(request.LobbyInstanceId, out MatchRecord? existing))
            {
                EnsureSameRequest(existing.Session, request);
                result = existing.Session;
            }
            else
            {
                MatchRecord created = CreateRecord(request);
                _matchesByLobby.Add(request.LobbyInstanceId, created);
                _matchesById.Add(created.Session.MatchId, created);
                result = created.Session;
            }
        }

        return result;
    }

    public MatchBoardSnapshot? GetBoard(Guid matchId)
    {
        MatchBoardSnapshot? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.ToBoard();
            }
        }

        return result;
    }

    public MatchPrivateHand? GetPrivateHand(Guid matchId, Guid playerId)
    {
        MatchPrivateHand? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.ToPrivateHand(playerId);
            }
        }

        return result;
    }

    public MatchPlacementResult TryPlace(Guid matchId, Guid playerId, MatchPlacement move)
    {
        ArgumentNullException.ThrowIfNull(move);
        MatchPlacementResult result = MatchPlacementResult.MatchUnavailable;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.TryPlace(playerId, move, _timeProvider.GetUtcNow());
            }
        }

        return result;
    }

    public MatchDismantleResult TryDismantle(Guid matchId, Guid playerId, MatchDismantle move)
    {
        ArgumentNullException.ThrowIfNull(move);
        MatchDismantleResult result = MatchDismantleResult.MatchUnavailable;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.TryDismantle(playerId, move, _timeProvider.GetUtcNow());
            }
        }

        return result;
    }

    public MatchLifecycleSnapshot? Get(Guid matchId)
    {
        MatchLifecycleSnapshot? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.Lifecycle;
            }
        }

        return result;
    }

    public bool TryFinish(Guid matchId)
    {
        bool result = TryTransition(matchId, MatchLifecycleStatus.Finished);

        return result;
    }

    public bool TryCancel(Guid matchId)
    {
        bool result = TryTransition(matchId, MatchLifecycleStatus.Cancelled);

        return result;
    }

    public MatchPresenceSnapshot? GetPresence(Guid matchId)
    {
        MatchPresenceSnapshot? result = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.ToPresence();
            }
        }

        return result;
    }

    public bool TryDisconnect(Guid matchId, Guid playerId)
    {
        return TrySetConnected(matchId, playerId, false);
    }

    public MatchReconnectResult TryReconnect(Guid matchId, Guid playerId)
    {
        MatchReconnectResult result = MatchReconnectResult.Rejected;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = record.TryReconnect(playerId, _timeProvider.GetUtcNow());
            }
        }

        return result;
    }

    private bool TrySetConnected(Guid matchId, Guid playerId, bool isConnected)
    {
        bool changed = false;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                changed = record.TrySetConnected(playerId, isConnected, _timeProvider.GetUtcNow());
            }
        }

        return changed;
    }

    public int PurgeTerminated(TimeSpan retention)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(retention.Ticks);
        int removed = 0;
        DateTimeOffset now = _timeProvider.GetUtcNow();
        TimeSpan maximumSubtractable = now - DateTimeOffset.MinValue;
        if (retention <= maximumSubtractable)
        {
            DateTimeOffset cutoff = now - retention;
            lock (_syncRoot)
            {
                Guid[] expiredIds = _matchesById.Values
                    .Where(record => record.IsTerminal && record.Lifecycle.UpdatedAtUtc <= cutoff)
                    .Select(record => record.Session.MatchId)
                    .ToArray();
                foreach (Guid matchId in expiredIds)
                {
                    MatchRecord record = _matchesById[matchId];
                    _matchesById.Remove(matchId);
                    _matchesByLobby.Remove(record.Session.LobbyInstanceId);
                    removed++;
                }
            }
        }

        return removed;
    }

    private bool TryTransition(Guid matchId, MatchLifecycleStatus next)
    {
        bool changed = false;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                changed = record.TryTransition(next, _timeProvider.GetUtcNow());
            }
        }

        return changed;
    }

    private MatchRecord CreateRecord(MatchStartRequest request)
    {
        Guid[] players = request.PlayerIds.ToArray();
        DateTimeOffset now = _timeProvider.GetUtcNow();
        MatchSessionSnapshot session = new(
            Guid.NewGuid(), request.LobbyInstanceId, request.LobbyCode,
            request.HostPlayerId, Array.AsReadOnly(players), now.UtcDateTime);
        MatchRecord record = new(session, now);

        return record;
    }

    private static void Validate(MatchStartRequest request)
    {
        if (request.LobbyInstanceId == Guid.Empty || string.IsNullOrWhiteSpace(request.LobbyCode)
            || request.HostPlayerId == Guid.Empty || request.PlayerIds is null
            || request.PlayerIds.Count is < MinimumPlayerCount or > MaximumPlayerCount
            || request.PlayerIds.Distinct().Count() != request.PlayerIds.Count
            || !request.PlayerIds.Contains(request.HostPlayerId) || request.PlayerIds.Any(id => id == Guid.Empty))
        {
            throw new ArgumentException("Invalid match initialization data.", nameof(request));
        }
    }

    private static void EnsureSameRequest(MatchSessionSnapshot existing, MatchStartRequest request)
    {
        if (existing.LobbyCode != request.LobbyCode || existing.InitialHostPlayerId != request.HostPlayerId
            || !existing.PlayerIds.SequenceEqual(request.PlayerIds))
        {
            throw new InvalidOperationException("A match already exists for a different lobby snapshot.");
        }
    }
}
