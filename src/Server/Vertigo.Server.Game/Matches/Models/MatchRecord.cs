using System.Security.Cryptography;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Models;

internal sealed class MatchRecord
{
    private const int MaximumTowerCount = 6;
    private const int FirstRoundNumber = 1;
    private const int FinalRoundNumber = 3;
    private const int MaximumFloorsPerDismantle = 10;
    private const int MinimumBlocksForMasterPlan = 3;
    private const int MaximumRecentMoves = 4;
    private const long InitialBoardRevision = 1;

    private static readonly TimeSpan TurnDuration = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan PresenceGraceDuration = TimeSpan.FromSeconds(30);

    private readonly Dictionary<Guid, DateTimeOffset> _presenceDeadlinesUtc = new();
    private readonly List<Guid> _inactivePlayerIds = new();
    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private readonly HashSet<Guid> _activePlayerIds;
    private readonly Dictionary<Guid, HashSet<Guid>> _expulsionVotes = new();
    private readonly Dictionary<Guid, IReadOnlyList<int>> _hands = new();
    private readonly Dictionary<Guid, int> _personalFloors = new();
    private readonly Dictionary<Guid, int> _masterPlans = new();
    private readonly MatchScoring _scoring = new();
    private readonly Func<MatchDeck> _createDeck;
    private readonly List<MatchBoardPile> _piles = new();
    private readonly Queue<MatchBoardMove> _recentMoves = new();
    private MatchDeck _deck;
    private int _roundNumber = FirstRoundNumber;
    private Guid _roundOpenerId;
    private Guid _currentTurnPlayerId;
    private DateTimeOffset? _turnDeadlineUtc;
    private MatchEndReason _endReason;
    private Guid[] _winnerIds = Array.Empty<Guid>();
    private long _boardRevision = InitialBoardRevision;
    private long _latestGameplayRevision = InitialBoardRevision;
    private long _votingRevision;
    private DateTimeOffset? _recoveryPausedAt;

    private int ExpulsionVotesRequired => _activePlayerIds.Count / 2 + 1;

    public MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc)
        : this(session, createdAtUtc, (() => new MatchDeck(), RandomNumberGenerator.GetInt32))
    {
    }

    internal MatchRecord(
        MatchSessionSnapshot session,
        DateTimeOffset createdAtUtc,
        (Func<MatchDeck> CreateDeck, Func<int, int> PickOpeningPlayer) setup)
    {
        Session = session;
        _activePlayerIds = new HashSet<Guid>(session.PlayerIds);
        _createDeck = setup.CreateDeck;
        _deck = _createDeck();
        Guid opener = session.PlayerIds[setup.PickOpeningPlayer(session.PlayerIds.Count)];
        DealRound(opener, createdAtUtc);
        Lifecycle = new MatchLifecycleSnapshot
        {
            MatchId = session.MatchId,
            LobbyInstanceId = session.LobbyInstanceId,
            Status = MatchLifecycleStatus.Running,
            Revision = 1,
            UpdatedAtUtc = createdAtUtc
        };
    }

    private void DealRound(Guid opener, DateTimeOffset now)
    {
        foreach (Guid playerId in Session.PlayerIds.Where(_activePlayerIds.Contains))
        {
            _hands[playerId] = _deck.Draw(MatchDeck.InitialHandSize);
            _personalFloors[playerId] = 0;
            _masterPlans[playerId] = 0;
        }

        _piles.Clear();
        AddTower();

        _roundOpenerId = opener;
        BeginTurn(opener, now);
    }

    public MatchSessionSnapshot Session { get; }

    public long BoardRevision => _boardRevision;

    public IReadOnlyList<Guid> ActivePlayers => Session.PlayerIds.Where(_activePlayerIds.Contains).ToArray();

    public MatchLifecycleSnapshot Lifecycle { get; private set; }

    public MatchBoardSnapshot ToBoard()
    {
        Guid[] activePlayers = Session.PlayerIds.Where(_activePlayerIds.Contains).ToArray();
        Dictionary<Guid, int>? positions = _endReason == MatchEndReason.Completed
            ? _scoring.GetFinalPositions(activePlayers) : null;

        return new MatchBoardSnapshot
        {
            MatchId = Session.MatchId,
            Revision = _boardRevision,
            RoundNumber = _roundNumber,
            CurrentTurnPlayerId = _currentTurnPlayerId,
            TurnDeadlineUtc = _turnDeadlineUtc,
            EndReason = _endReason,
            PresenceDeadlinesUtc = new(_presenceDeadlinesUtc),
            InactivePlayerIds = new(_inactivePlayerIds),
            ExpelledPlayerIds = Session.PlayerIds.Where(id => !_activePlayerIds.Contains(id)).ToList(),
            VotingRevision = _votingRevision,
            ExpulsionVotesRequired = ExpulsionVotesRequired,
            ExpulsionVotes = Session.PlayerIds.Where(_expulsionVotes.ContainsKey).Select(id =>
                new MatchExpulsionVoteSummary { TargetPlayerId = id, VoteCount = _expulsionVotes[id].Count }).ToList(),
            WinnerIds = _winnerIds.ToList(),
            RoundSummaries = _scoring.ToRoundSummaries(),
            RecentMoves = _recentMoves.Select(move => new MatchBoardMove
            {
                Revision = move.Revision,
                PlayerId = move.PlayerId,
                ActionKind = move.ActionKind,
                PilePosition = move.PilePosition,
                PreviousTop = move.PreviousTop,
                CardNumbers = move.CardNumbers.ToList()
            }).ToList(),
            Players = activePlayers.Select(id => new MatchBoardPlayer
            {
                PlayerId = id,
                FloorsBuilt = _personalFloors[id],
                GlobalFloors = _scoring.GetGlobalFloors(id),
                FinalPosition = _endReason == MatchEndReason.Abandonment ? 1 : positions?[id]
            }).ToList(),
            Piles = _piles.Select(pile => new MatchBoardPile
            {
                Position = pile.Position,
                TopCardNumber = pile.TopCardNumber,
                CardCount = pile.CardCount
            }).ToList()
        };
    }

    public MatchPlacementResult TryPlace(Guid playerId, MatchPlacement move, DateTimeOffset now)
    {
        return GetTurnRejection(playerId, move.ExpectedRevision, now) ?? TryApplyPlacement(playerId, move, now);
    }

    private MatchPlacementResult? GetTurnRejection(Guid playerId, long expectedRevision, DateTimeOffset now)
    {
        MatchPlacementResult? rejection = null;
        if (Lifecycle.Status != MatchLifecycleStatus.Running)
        {
            rejection = MatchPlacementResult.MatchUnavailable;
        }
        else if (IsTurnExpired(now))
        {
            rejection = MatchPlacementResult.TurnExpired;
        }
        else if (expectedRevision < _latestGameplayRevision || expectedRevision > _boardRevision)
        {
            rejection = MatchPlacementResult.StaleRevision;
        }
        else if (playerId != _currentTurnPlayerId)
        {
            rejection = MatchPlacementResult.NotYourTurn;
        }

        return rejection;
    }

    private MatchPlacementResult TryApplyPlacement(Guid playerId, MatchPlacement move, DateTimeOffset now)
    {
        MatchPlacementResult result;
        if (!_hands.TryGetValue(playerId, out IReadOnlyList<int>? cards) || !cards.Contains(move.CardNumber))
        {
            result = MatchPlacementResult.CardNotOwned;
        }
        else if (move.PilePosition < 1 || move.PilePosition > _piles.Count)
        {
            result = MatchPlacementResult.InvalidPile;
        }
        else
        {
            MatchBoardPile pile = _piles[move.PilePosition - 1];
            if (!pile.TopCardNumber.HasValue
                || !MatchPlacementRules.IsValid(pile.TopCardNumber.Value, move.CardNumber))
            {
                result = MatchPlacementResult.InvalidPlacement;
            }
            else
            {
                ApplyModules(playerId, pile, new[] { move.CardNumber });
                CompleteTurn(playerId, now);
                result = MatchPlacementResult.Applied;
            }
        }

        return result;
    }

    public MatchPlacementResult TryUseMasterPlan(Guid playerId, MatchMasterPlan move, DateTimeOffset now)
    {
        return GetTurnRejection(playerId, move.ExpectedRevision, now) ?? TryApplyMasterPlan(playerId, move, now);
    }

    private MatchPlacementResult TryApplyMasterPlan(Guid playerId, MatchMasterPlan move, DateTimeOffset now)
    {
        IReadOnlyList<int> hand = _hands[playerId];
        IReadOnlyList<int> modules = move.CardNumbers;
        MatchPlacementResult result;
        if (_masterPlans[playerId] == 0)
        {
            result = MatchPlacementResult.MasterPlanRequired;
        }
        else if (modules is null || modules.Count < 2 || modules.Count > hand.Count
            || modules.Distinct().Count() != modules.Count)
        {
            result = MatchPlacementResult.InvalidMasterPlanChain;
        }
        else if (!modules.All(module => hand.Contains(module)))
        {
            result = MatchPlacementResult.CardNotOwned;
        }
        else if (move.PilePosition < 1 || move.PilePosition > _piles.Count)
        {
            result = MatchPlacementResult.InvalidPile;
        }
        else
        {
            result = PlayMasterPlan(playerId, _piles[move.PilePosition - 1], modules);
            if (result == MatchPlacementResult.Applied)
            {
                CompleteTurn(playerId, now);
            }
        }

        return result;
    }

    private MatchPlacementResult PlayMasterPlan(Guid playerId, MatchBoardPile pile, IReadOnlyList<int> modules)
    {
        MatchPlacementResult result = MatchPlacementResult.InvalidMasterPlanChain;
        if (pile.TopCardNumber is int top && IsValidChain(top, modules))
        {
            ApplyModules(playerId, pile, modules);
            _masterPlans[playerId]--;
            result = MatchPlacementResult.Applied;
        }

        return result;
    }

    private static bool IsValidChain(int top, IReadOnlyList<int> modules)
    {
        bool valid = true;
        for (int index = 0; index < modules.Count && valid; index++)
        {
            int module = modules[index];
            valid = MatchPlacementRules.IsValid(top, module);
            top = module;
        }

        return valid;
    }

    private void ApplyModules(Guid playerId, MatchBoardPile pile, IReadOnlyList<int> modules)
    {
        MatchBoardActionKind kind = modules.Count == 1
            ? MatchBoardActionKind.Placement : MatchBoardActionKind.MasterPlan;
        RecordMove(playerId, (kind, pile.Position, pile.TopCardNumber), modules);
        _hands[playerId] = _hands[playerId].Where(card => !modules.Contains(card)).ToArray();
        pile.TopCardNumber = modules[^1];
        pile.CardCount += modules.Count;
    }

    private void RecordMove(Guid playerId, (MatchBoardActionKind Kind, int PilePosition, int? PreviousTop) action,
        IReadOnlyList<int> modules)
    {
        if (_recentMoves.Count == MaximumRecentMoves)
        {
            _recentMoves.Dequeue();
        }

        _recentMoves.Enqueue(new MatchBoardMove
        {
            Revision = _boardRevision + 1,
            PlayerId = playerId,
            ActionKind = action.Kind,
            PilePosition = action.PilePosition,
            PreviousTop = action.PreviousTop,
            CardNumbers = modules.ToList()
        });
    }

    public MatchDismantleResult TryDismantle(Guid playerId, MatchDismantle move, DateTimeOffset now)
    {
        MatchDismantleResult result = GetTurnRejection(playerId, move.ExpectedRevision, now) switch
        {
            MatchPlacementResult.MatchUnavailable => MatchDismantleResult.MatchUnavailable,
            MatchPlacementResult.StaleRevision => MatchDismantleResult.StaleRevision,
            MatchPlacementResult.TurnExpired => MatchDismantleResult.TurnExpired,
            MatchPlacementResult.NotYourTurn => MatchDismantleResult.NotYourTurn,
            null => TryApplyDismantle(playerId, move, now),
            _ => throw new InvalidOperationException("Unexpected turn rejection.")
        };

        return result;
    }

    private MatchDismantleResult TryApplyDismantle(Guid playerId, MatchDismantle move, DateTimeOffset now)
    {
        MatchDismantleResult result;
        if (move.PilePosition < 1 || move.PilePosition > _piles.Count)
        {
            result = MatchDismantleResult.InvalidPile;
        }
        else
        {
            MatchBoardPile pile = _piles[move.PilePosition - 1];
            if (pile.TopCardNumber is null)
            {
                result = MatchDismantleResult.EmptyPile;
            }
            else if (CanPlaceAnyModule(playerId))
            {
                result = MatchDismantleResult.PlacementAvailable;
            }
            else
            {
                RecordMove(playerId, (MatchBoardActionKind.Dismantle, pile.Position, pile.TopCardNumber),
                    Array.Empty<int>());
                DismantleTower(playerId, pile);
                CompleteTurn(playerId, now);
                result = MatchDismantleResult.Applied;
            }
        }

        return result;
    }

    private bool CanPlaceAnyModule(Guid playerId)
    {
        IReadOnlyList<int> hand = _hands[playerId];

        return _piles.Any(pile => pile.TopCardNumber is int top
            && hand.Any(module => MatchPlacementRules.IsValid(top, module)));
    }

    private void DismantleTower(Guid playerId, MatchBoardPile pile)
    {
        _personalFloors[playerId] += Math.Min(pile.CardCount, MaximumFloorsPerDismantle);
        if (pile.CardCount >= MinimumBlocksForMasterPlan)
        {
            _masterPlans[playerId]++;
        }

        RefillTower(pile);
        OpenAdditionalTower();
    }

    private void OpenAdditionalTower()
    {
        if (_deck.RemainingCount == 0)
        {
            return;
        }

        MatchBoardPile? vacant = _piles.Find(pile => pile.TopCardNumber is null);
        if (vacant is not null)
        {
            RefillTower(vacant);
        }
        else if (_piles.Count < MaximumTowerCount)
        {
            AddTower();
        }
    }

    private void AddTower()
    {
        MatchBoardPile pile = new() { Position = _piles.Count + 1 };
        RefillTower(pile);
        _piles.Add(pile);
    }

    private void RefillTower(MatchBoardPile pile)
    {
        pile.TopCardNumber = _deck.RemainingCount > 0 ? _deck.Draw(1)[0] : null;
        pile.CardCount = pile.TopCardNumber.HasValue ? 1 : 0;
    }

    public MatchDepartureResult TryLeaveMatch(Guid playerId, DateTimeOffset now)
    {
        bool accepted = Session.PlayerIds.Contains(playerId);
        long previousRevision = _boardRevision;
        if (accepted && !IsTerminal && _activePlayerIds.Contains(playerId))
        {
            TryExpireTurn(now);
            if (!IsTerminal && _activePlayerIds.Contains(playerId))
            {
                RemoveActivePlayer(playerId, MatchBoardActionKind.LeftMatch, now);
            }
        }

        return new MatchDepartureResult(accepted, _boardRevision != previousRevision);
    }

    public Guid? TryExpireTurn(DateTimeOffset now)
    {
        Guid? affectedPlayerId = null;
        if (!IsTerminal && !_recoveryPausedAt.HasValue)
        {
            Guid expiredPlayerId = _presenceDeadlinesUtc
                .Where(entry => entry.Value <= now)
                .OrderBy(entry => entry.Value)
                .Select(entry => entry.Key)
                .FirstOrDefault();
            if (expiredPlayerId != Guid.Empty)
            {
                _inactivePlayerIds.Add(expiredPlayerId);
                RemoveActivePlayer(expiredPlayerId, MatchBoardActionKind.TurnExpired, now);
                affectedPlayerId = expiredPlayerId;
            }
            else if (IsTurnExpired(now))
            {
                Guid skippedPlayerId = _currentTurnPlayerId;
                _presenceDeadlinesUtc.Add(skippedPlayerId, _turnDeadlineUtc!.Value + PresenceGraceDuration);
                RecordMove(skippedPlayerId, (MatchBoardActionKind.TurnSkipped, 0, null), Array.Empty<int>());
                AdvanceGameplayRevision();
                BeginTurn(GetActivePlayersAfter(skippedPlayerId).First(), now);
                affectedPlayerId = skippedPlayerId;
            }
        }

        return affectedPlayerId;
    }

    public bool TryConfirmPresence(Guid playerId, DateTimeOffset now)
    {
        bool confirmed = !IsTerminal && playerId != _currentTurnPlayerId
            && !_disconnectedPlayerIds.Contains(playerId)
            && _presenceDeadlinesUtc.TryGetValue(playerId, out DateTimeOffset deadline) && now < deadline;
        if (confirmed)
        {
            _presenceDeadlinesUtc.Remove(playerId);
            RecordMove(playerId, (MatchBoardActionKind.PresenceConfirmed, 0, null), Array.Empty<int>());
            _boardRevision++;
        }

        return confirmed;
    }

    public MatchExpulsionVoteResult TryVoteToExpel(Guid voterId, MatchExpulsionVote vote, DateTimeOffset now)
    {
        MatchExpulsionVoteResult result;
        if (Lifecycle.Status != MatchLifecycleStatus.Running)
        {
            result = MatchExpulsionVoteResult.MatchUnavailable;
        }
        else if (IsTurnExpired(now) || HasPresenceExpired(voterId, now) || HasPresenceExpired(vote.TargetPlayerId, now))
        {
            result = MatchExpulsionVoteResult.TurnExpired;
        }
        else if (vote.ExpectedVotingRevision != _votingRevision)
        {
            result = MatchExpulsionVoteResult.StaleVotingRevision;
        }
        else if (!_activePlayerIds.Contains(voterId))
        {
            result = MatchExpulsionVoteResult.InvalidVoter;
        }
        else if (vote.TargetPlayerId == voterId || !_activePlayerIds.Contains(vote.TargetPlayerId))
        {
            result = MatchExpulsionVoteResult.InvalidTarget;
        }
        else if (_activePlayerIds.Count <= ExpulsionVotesRequired)
        {
            result = MatchExpulsionVoteResult.TooFewPlayers;
        }
        else
        {
            result = RecordExpulsionVote(voterId, vote.TargetPlayerId, now);
        }

        return result;
    }

    private MatchExpulsionVoteResult RecordExpulsionVote(Guid voterId, Guid targetId, DateTimeOffset now)
    {
        if (!_expulsionVotes.TryGetValue(targetId, out HashSet<Guid>? voters))
        {
            voters = new HashSet<Guid>();
            _expulsionVotes.Add(targetId, voters);
        }

        MatchExpulsionVoteResult result = MatchExpulsionVoteResult.AlreadyVoted;
        if (voters.Add(voterId))
        {
            if (voters.Count >= ExpulsionVotesRequired)
            {
                RemoveActivePlayer(targetId, MatchBoardActionKind.ExpelledByVote, now);
                result = MatchExpulsionVoteResult.Expelled;
            }
            else
            {
                _boardRevision++;
                result = MatchExpulsionVoteResult.Recorded;
            }
        }

        return result;
    }

    private bool HasPresenceExpired(Guid playerId, DateTimeOffset now)
    {
        return _presenceDeadlinesUtc.TryGetValue(playerId, out DateTimeOffset deadline) && now >= deadline;
    }

    private bool IsTurnExpired(DateTimeOffset now)
    {
        return !IsTerminal && _turnDeadlineUtc is DateTimeOffset deadline && now >= deadline;
    }

    private void RemoveActivePlayer(Guid playerId, MatchBoardActionKind kind, DateTimeOffset now)
    {
        bool wasCurrentPlayer = _currentTurnPlayerId == playerId;
        RecordMove(playerId, (kind, 0, null), Array.Empty<int>());
        _deck.ReturnToBottom(_hands[playerId]);
        _hands.Remove(playerId);
        _activePlayerIds.Remove(playerId);
        _presenceDeadlinesUtc.Remove(playerId);
        _disconnectedPlayerIds.Remove(playerId);
        _expulsionVotes.Clear();
        _votingRevision++;
        AdvanceGameplayRevision();

        if (_activePlayerIds.Count == 1)
        {
            FinishMatch(_activePlayerIds.ToArray(), MatchEndReason.Abandonment, now);
        }
        else
        {
            ContinueRound(playerId, now, advanceTurn: wasCurrentPlayer);
            if (!IsTerminal)
            {
                Lifecycle = Lifecycle with
                {
                    Status = _disconnectedPlayerIds.Count == _activePlayerIds.Count
                        ? MatchLifecycleStatus.Suspended : MatchLifecycleStatus.Running,
                    Revision = Lifecycle.Revision + 1,
                    UpdatedAtUtc = now
                };
            }
        }
    }

    private void AdvanceGameplayRevision()
    {
        _boardRevision++;
        _latestGameplayRevision = _boardRevision;
    }

    private void CompleteTurn(Guid playerId, DateTimeOffset now)
    {
        _presenceDeadlinesUtc.Remove(playerId);
        AdvanceGameplayRevision();
        if (_hands[playerId].Count == 0)
        {
            CloseRound(playerId, now);
        }
        else
        {
            ContinueRound(playerId, now);
        }
    }

    private void ContinueRound(Guid after, DateTimeOffset now, bool advanceTurn = true)
    {
        if (_deck.RemainingCount == 0 && !_activePlayerIds.Any(CanPlaceAnyModule))
        {
            CloseRound(null, now);
        }
        else if (advanceTurn)
        {
            BeginTurn(GetActivePlayersAfter(after).First(), now);
        }
    }

    private void BeginTurn(Guid playerId, DateTimeOffset now)
    {
        _currentTurnPlayerId = playerId;
        _turnDeadlineUtc = _presenceDeadlinesUtc.TryGetValue(playerId, out DateTimeOffset deadline)
            ? deadline : now + TurnDuration;
    }

    private void CloseRound(Guid? closedBy, DateTimeOffset now)
    {
        Dictionary<Guid, int> floors = _activePlayerIds.ToDictionary(playerId => playerId,
            playerId => _personalFloors[playerId]);
        _scoring.AddRound(floors, closedBy);
        if (_roundNumber == FinalRoundNumber)
        {
            IReadOnlyList<Guid> players = Session.PlayerIds.Where(_activePlayerIds.Contains).ToArray();
            FinishMatch(_scoring.GetWinners(players), MatchEndReason.Completed, now);
        }
        else
        {
            Guid opener = GetActivePlayersAfter(_roundOpenerId).MinBy(_scoring.GetGlobalFloors);
            _roundNumber++;
            _deck = _createDeck();
            DealRound(opener, now);
        }
    }

    private IEnumerable<Guid> GetActivePlayersAfter(Guid playerId)
    {
        IReadOnlyList<Guid> players = Session.PlayerIds;
        int previous = players.ToList().IndexOf(playerId);

        return Enumerable.Range(1, players.Count)
            .Select(offset => players[(previous + offset) % players.Count])
            .Where(_activePlayerIds.Contains);
    }

    private void FinishMatch(Guid[] winnerIds, MatchEndReason reason, DateTimeOffset now)
    {
        _winnerIds = winnerIds;
        _endReason = reason;
        _currentTurnPlayerId = Guid.Empty;
        TryTransition(MatchLifecycleStatus.Finished, now);
    }

    public MatchPrivateHand? ToPrivateHand(Guid playerId)
    {
        MatchPrivateHand? hand = null;
        if (_hands.TryGetValue(playerId, out IReadOnlyList<int>? cards))
        {
            hand = new MatchPrivateHand
            {
                MatchId = Session.MatchId,
                PlayerId = playerId,
                Cards = cards.ToList(),
                MasterPlans = _masterPlans[playerId],
                Revision = _boardRevision,
                VotingRevision = _votingRevision,
                VotedExpulsionTargetIds = Session.PlayerIds.Where(targetId =>
                    _expulsionVotes.TryGetValue(targetId, out HashSet<Guid>? voters) && voters.Contains(playerId))
                    .ToList()
            };
        }

        return hand;
    }

    public MatchPresenceSnapshot ToPresence()
    {
        return new MatchPresenceSnapshot
        {
            MatchId = Session.MatchId,
            Status = Lifecycle.Status,
            Revision = Lifecycle.Revision,
            DisconnectedPlayerIds = _disconnectedPlayerIds.OrderBy(id => id).ToArray()
        };
    }

    public bool TrySetConnected(Guid playerId, bool isConnected, DateTimeOffset now)
    {
        bool changed = false;
        if (!IsTerminal && _activePlayerIds.Contains(playerId))
        {
            changed = isConnected
                ? _disconnectedPlayerIds.Remove(playerId)
                : _disconnectedPlayerIds.Add(playerId);
            if (changed)
            {
                MatchLifecycleStatus status = _disconnectedPlayerIds.Count == _activePlayerIds.Count
                    ? MatchLifecycleStatus.Suspended : MatchLifecycleStatus.Running;
                Lifecycle = Lifecycle with
                {
                    Status = status,
                    Revision = Lifecycle.Revision + 1,
                    UpdatedAtUtc = now
                };
            }
        }

        return changed;
    }

    public MatchReconnectResult TryReconnect(Guid playerId, DateTimeOffset now)
    {
        MatchReconnectResult result = MatchReconnectResult.Rejected;
        if (!IsTerminal && _activePlayerIds.Contains(playerId)
            && !HasPresenceExpired(playerId, _recoveryPausedAt ?? now))
        {
            if (_disconnectedPlayerIds.Remove(playerId))
            {
                ResumeAfterRecovery(now);
                Lifecycle = Lifecycle with
                {
                    Status = MatchLifecycleStatus.Running,
                    Revision = Lifecycle.Revision + 1,
                    UpdatedAtUtc = now
                };
                result = MatchReconnectResult.Reconnected;
            }
            else
            {
                result = MatchReconnectResult.AlreadyConnected;
            }
        }

        return result;
    }

    public bool PauseForRecovery(DateTimeOffset lastEvent)
    {
        bool paused = !IsTerminal && !_recoveryPausedAt.HasValue;
        if (paused)
        {
            _disconnectedPlayerIds.UnionWith(_activePlayerIds);
            _recoveryPausedAt = lastEvent;
            _turnDeadlineUtc = null;
            AdvanceGameplayRevision();
            Lifecycle = Lifecycle with
            {
                Status = MatchLifecycleStatus.Suspended,
                Revision = Lifecycle.Revision + 1,
                UpdatedAtUtc = lastEvent
            };
        }

        return paused;
    }

    private void ResumeAfterRecovery(DateTimeOffset now)
    {
        if (_recoveryPausedAt is DateTimeOffset pausedAt)
        {
            TimeSpan elapsed = now >= pausedAt ? now - pausedAt : TimeSpan.Zero;
            foreach (Guid playerId in _presenceDeadlinesUtc.Keys.ToArray())
            {
                _presenceDeadlinesUtc[playerId] += elapsed;
            }

            _recoveryPausedAt = null;
            BeginTurn(_currentTurnPlayerId, now);
            AdvanceGameplayRevision();
        }
    }

    public bool IsTerminal => Lifecycle.Status is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled;

    public bool TryTransition(MatchLifecycleStatus next, DateTimeOffset changedAtUtc)
    {
        bool changed = false;
        if (!IsTerminal && (next is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled))
        {
            _turnDeadlineUtc = null;
            _presenceDeadlinesUtc.Clear();
            _expulsionVotes.Clear();
            Lifecycle = Lifecycle with
            {
                Status = next,
                Revision = Lifecycle.Revision + 1,
                UpdatedAtUtc = changedAtUtc
            };
            changed = true;
        }

        return changed;
    }
}
