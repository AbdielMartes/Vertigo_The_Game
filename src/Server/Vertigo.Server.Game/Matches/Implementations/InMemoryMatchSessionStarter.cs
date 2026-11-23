using System.Text.Json;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.Game.Matches.Recovery;
using Vertigo.Contracts.Internal;
using Vertigo.Server.Game.Matches.Models;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Implementations;

// One synchronized in-memory registry for idempotent starts and terminal transitions.
// Deliberately no automatic cancellation when lobby connections disappear (Phase 7B).
public sealed class InMemoryMatchSessionStarter :
    IMatchSessionStarter, IMatchLifecycle, IMatchPresence, IMatchStateReader, IMatchCommandService, IMatchRecovery,
    ICompletedMatchSource
{
    private const int MinimumPlayerCount = 2;
    private const int MaximumPlayerCount = 6;

    private readonly object _syncRoot = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesByLobby = new();
    private readonly Dictionary<Guid, MatchRecord> _matchesById = new();
    private readonly OrderedDictionary<Guid, CompletedMatch> _pendingResults = new();
    private readonly TimeProvider _timeProvider;

    private readonly MatchJournal? _journal;
    private readonly Dictionary<Guid, (MatchJournal.Header Header, int Sequence, DateTimeOffset LastEvent)> _journalPositions = new();
    private readonly List<(MatchSessionSnapshot Session, RecoverableLobby Lobby, IReadOnlyList<Guid> ActivePlayers)>
        _recoveredMatches = new();

    public InMemoryMatchSessionStarter(TimeProvider timeProvider, RecoveryDirectory? recovery = null)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
        if (recovery is not null)
        {
            _journal = new MatchJournal(recovery);
            RecoverActiveMatches();
        }
    }

    public IReadOnlyList<(MatchSessionSnapshot Session, RecoverableLobby Lobby, IReadOnlyList<Guid> ActivePlayers)>
        GetRecoveredMatches()
    {
        return _recoveredMatches.AsReadOnly();
    }

    private void RecoverActiveMatches()
    {
        foreach (MatchJournal.Header header in _journal!.ReadStartedMatches())
        {
            MatchRecord record = Replay(header);
            if (!record.IsTerminal)
            {
                Guid id = header.Session.MatchId;
                _matchesById.Add(id, record);
                _matchesByLobby.Add(header.Session.LobbyInstanceId, record);
                DateTimeOffset lastEvent = _journalPositions[id].LastEvent;
                Apply(record, new(lastEvent, new(nameof(MatchRecord.PauseForRecovery), Guid.Empty)),
                    record.PauseForRecovery);
                if (header.Lobby is not null)
                {
                    _recoveredMatches.Add((header.Session, header.Lobby, record.ActivePlayers));
                }
            }
            else
            {
                if (!_journal.AreStatisticsRecorded(header.Session.MatchId))
                {
                    TrackCompletedResult(record, header.Lobby);
                }

                _journalPositions.Remove(header.Session.MatchId);
            }
        }
    }

    private MatchRecord Replay(MatchJournal.Header header)
    {
        MatchRecord record = MatchJournal.CreateRecord(header);
        int sequence = 0;
        DateTimeOffset lastEvent = header.Session.CreatedAtUtc;
        foreach (MatchJournal.Commit commit in _journal!.ReadEvents(header.Session.MatchId))
        {
            MatchJournal.Apply(record, commit.Entry);
            if (record.BoardRevision != commit.BoardRevision || record.Lifecycle.Revision != commit.LifecycleRevision)
            {
                throw new InvalidDataException("The match event replay produced a different revision.");
            }

            sequence++;
            lastEvent = commit.Entry.Time;
        }

        _journalPositions[header.Session.MatchId] = (header, sequence, lastEvent);
        return record;
    }

    private T Apply<T>(MatchRecord record, MatchJournal.Entry entry, Func<DateTimeOffset, T> operation)
    {
        Guid id = record.Session.MatchId;
        bool wasTerminal = record.IsTerminal;
        (long Board, long Lifecycle) previous = (record.BoardRevision, record.Lifecycle.Revision);
        T result;
        try
        {
            result = operation(entry.Time);
            if (_journal is not null && previous != (record.BoardRevision, record.Lifecycle.Revision))
            {
                (MatchJournal.Header header, int sequence, _) = _journalPositions[id];
                _journal.Append(id, sequence + 1,
                    new MatchJournal.Commit(entry, record.BoardRevision, record.Lifecycle.Revision));
                _journalPositions[id] = (header, sequence + 1, entry.Time);
            }
        }
        catch
        {
            if (_journal is not null)
            {
                MatchRecord restored = Replay(_journalPositions[id].Header);
                _matchesById[id] = restored;
                _matchesByLobby[record.Session.LobbyInstanceId] = restored;
            }

            throw;
        }

        if (!wasTerminal && record.IsTerminal)
        {
            TrackCompletedResult(record, _journalPositions.GetValueOrDefault(id).Header?.Lobby);
        }

        return result;
    }

    public IReadOnlyList<CompletedMatch> GetPendingResults(int limit)
    {
        lock (_syncRoot)
        {
            return _pendingResults.Values.Take(limit).ToArray();
        }
    }

    public void AcknowledgeResult(Guid matchId)
    {
        _journal?.AcknowledgeStatistics(matchId);
        lock (_syncRoot)
        {
            _pendingResults.Remove(matchId);
        }
    }

    public void DeferResult(Guid matchId)
    {
        lock (_syncRoot)
        {
            if (_pendingResults.Remove(matchId, out CompletedMatch? result))
            {
                _pendingResults.Add(matchId, result);
            }
        }
    }

    private void TrackCompletedResult(MatchRecord record, RecoverableLobby? lobby)
    {
        MatchBoardSnapshot board = record.ToBoard();
        if (board.EndReason == MatchEndReason.Completed)
        {
            HashSet<Guid>? registered = lobby?.Members.Where(member => !member.IsGuest)
                .Select(member => member.PlayerId).ToHashSet();
            var participants = board.Players.Where(player => registered is null || registered.Contains(player.PlayerId))
                .Select(player => BuildPlayerResult(board, player.PlayerId)).ToArray();
            if (participants.Length > 0)
            {
                _pendingResults[board.MatchId] = new CompletedMatch(board.MatchId,
                    record.Lifecycle.UpdatedAtUtc.UtcDateTime, board.RoundSummaries.Count,
                    board.Players.Max(player => player.GlobalFloors),
                    board.WinnerIds.Count == 1 ? board.WinnerIds[0] : null, participants);
            }
        }
    }

    private static CompletedMatchPlayer BuildPlayerResult(MatchBoardSnapshot board, Guid playerId)
    {
        int[] floors = board.RoundSummaries.SelectMany(round => round.Players)
            .Where(score => score.PlayerId == playerId).Select(score => score.FloorsBuilt).ToArray();
        return new CompletedMatchPlayer(playerId, board.WinnerIds.Contains(playerId),
            floors.Sum(), floors.DefaultIfEmpty().Max());
    }

    private MatchJournal.Entry Entry(string action, Guid player, object? payload = null)
    {
        JsonElement? data = _journal is not null && payload is not null
            ? JsonSerializer.SerializeToElement(payload)
            : null;
        return new MatchJournal.Entry(_timeProvider.GetUtcNow(), new MatchJournal.Action(action, player, data));
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
                result.ServerTimeUtc = _timeProvider.GetUtcNow();
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
                result = Apply(record, Entry(nameof(MatchRecord.TryPlace), playerId, move),
                    now => record.TryPlace(playerId, move, now));
            }
        }

        return result;
    }

    public MatchPlacementResult TryUseMasterPlan(Guid matchId, Guid playerId, MatchMasterPlan move)
    {
        ArgumentNullException.ThrowIfNull(move);
        MatchPlacementResult result = MatchPlacementResult.MatchUnavailable;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = Apply(record, Entry(nameof(MatchRecord.TryUseMasterPlan), playerId, move),
                    now => record.TryUseMasterPlan(playerId, move, now));
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
                result = Apply(record, Entry(nameof(MatchRecord.TryDismantle), playerId, move),
                    now => record.TryDismantle(playerId, move, now));
            }
        }

        return result;
    }

    public MatchExpulsionVoteResult TryVoteToExpel(Guid matchId, Guid voterId, MatchExpulsionVote vote)
    {
        ArgumentNullException.ThrowIfNull(vote);
        MatchExpulsionVoteResult result = MatchExpulsionVoteResult.MatchUnavailable;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = Apply(record, Entry(nameof(MatchRecord.TryVoteToExpel), voterId, vote),
                    now => record.TryVoteToExpel(voterId, vote, now));
            }
        }

        return result;
    }

    public MatchDepartureResult TryLeaveMatch(Guid matchId, Guid playerId)
    {
        MatchDepartureResult result = default;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                result = Apply(record, Entry(nameof(MatchRecord.TryLeaveMatch), playerId),
                    now => record.TryLeaveMatch(playerId, now));
            }
        }

        return result;
    }

    public Guid? TryExpireTurn(Guid matchId)
    {
        Guid? affectedPlayerId = null;
        lock (_syncRoot)
        {
            if (_matchesById.TryGetValue(matchId, out MatchRecord? record))
            {
                affectedPlayerId = Apply(record, Entry(nameof(MatchRecord.TryExpireTurn), Guid.Empty), record.TryExpireTurn);
            }
        }

        return affectedPlayerId;
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

    public bool TryConfirmPresence(Guid matchId, Guid playerId)
    {
        lock (_syncRoot)
        {
            return _matchesById.TryGetValue(matchId, out MatchRecord? record)
                && Apply(record, Entry(nameof(MatchRecord.TryConfirmPresence), playerId),
                    now => record.TryConfirmPresence(playerId, now));
        }
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
                result = Apply(record, Entry(nameof(MatchRecord.TryReconnect), playerId),
                    now => record.TryReconnect(playerId, now));
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
                changed = Apply(record, Entry(nameof(MatchRecord.TrySetConnected), playerId),
                    now => record.TrySetConnected(playerId, isConnected, now));
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
                    _journalPositions.Remove(matchId);
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
                changed = Apply(record, Entry(nameof(MatchRecord.TryTransition), Guid.Empty, next),
                    now => record.TryTransition(next, now));
            }
        }

        return changed;
    }

    private MatchRecord CreateRecord(MatchStartRequest request)
    {
        Guid[] players = request.PlayerIds.ToArray();
        DateTimeOffset now = _timeProvider.GetUtcNow();
        MatchSessionSnapshot session = new()
        {
            MatchId = Guid.NewGuid(),
            LobbyInstanceId = request.LobbyInstanceId,
            LobbyCode = request.LobbyCode,
            InitialHostPlayerId = request.HostPlayerId,
            PlayerIds = Array.AsReadOnly(players),
            CreatedAtUtc = now.UtcDateTime
        };
        MatchRecord record;
        if (_journal is null)
        {
            record = new MatchRecord(session, now);
        }
        else
        {
            MatchJournal.Header header = new(session, request.Lobby, Guid.NewGuid());
            record = MatchJournal.CreateRecord(header);
            _journal.Start(header);
            _journalPositions.Add(session.MatchId, (header, 0, now));
        }

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
