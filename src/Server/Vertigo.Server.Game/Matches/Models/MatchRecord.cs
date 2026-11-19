using System.Security.Cryptography;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Models;

// Protected by the owning starter's synchronization lock.
internal sealed class MatchRecord
{
    private const int InitialTowerCount = 4;
    private const int FirstRoundNumber = 1;
    private const int FinalRoundNumber = 3;
    private const int MaximumFloorsPerDismantle = 10;
    private const int MinimumBlocksForMasterPlan = 3;
    private const long InitialBoardRevision = 1;

    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private readonly Dictionary<Guid, IReadOnlyList<int>> _hands = new();
    private readonly Dictionary<Guid, int> _personalFloors = new();
    private readonly Dictionary<Guid, int> _masterPlans = new();
    private readonly MatchScoring _scoring = new();
    private readonly Func<MatchDeck> _createDeck;
    private readonly List<MatchBoardPile> _piles = new();
    private MatchDeck _deck;
    private int _roundNumber = FirstRoundNumber;
    private Guid _roundOpenerId;
    private Guid _currentTurnPlayerId;
    private Guid[] _winnerIds = Array.Empty<Guid>();
    private long _boardRevision = InitialBoardRevision;

    public MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc)
        : this(session, createdAtUtc, () => new MatchDeck())
    {
    }

    internal MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc, Func<MatchDeck> createDeck)
    {
        Session = session;
        _createDeck = createDeck;
        _deck = _createDeck();
        Guid opener = session.PlayerIds[RandomNumberGenerator.GetInt32(session.PlayerIds.Count)];
        DealRound(opener);
        Lifecycle = new MatchLifecycleSnapshot
        {
            MatchId = session.MatchId,
            LobbyInstanceId = session.LobbyInstanceId,
            Status = MatchLifecycleStatus.Running,
            Revision = 1,
            UpdatedAtUtc = createdAtUtc
        };
    }

    private void DealRound(Guid opener)
    {
        foreach (Guid playerId in Session.PlayerIds)
        {
            _hands[playerId] = _deck.Draw(MatchDeck.InitialHandSize);
            _personalFloors[playerId] = 0;
            _masterPlans[playerId] = 0;
        }

        IReadOnlyList<int> foundations = _deck.Draw(InitialTowerCount);
        _piles.Clear();
        for (int position = 1; position <= InitialTowerCount; position++)
        {
            _piles.Add(new MatchBoardPile
            {
                Position = position,
                TopCardNumber = foundations[position - 1],
                CardCount = 1
            });
        }

        _roundOpenerId = opener;
        _currentTurnPlayerId = opener;
    }

    public MatchSessionSnapshot Session { get; }

    public MatchLifecycleSnapshot Lifecycle { get; private set; }

    public MatchBoardSnapshot ToBoard()
    {
        return new MatchBoardSnapshot
        {
            MatchId = Session.MatchId,
            Revision = _boardRevision,
            RoundNumber = _roundNumber,
            CurrentTurnPlayerId = _currentTurnPlayerId,
            WinnerIds = _winnerIds.ToList(),
            Players = Session.PlayerIds.Select(id => new MatchBoardPlayer
            {
                PlayerId = id,
                FloorsBuilt = _personalFloors[id],
                GlobalFloors = _scoring.GetGlobalFloors(id)
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
        return GetTurnRejection(playerId, move.ExpectedRevision) ?? TryApplyPlacement(playerId, move, now);
    }

    private MatchPlacementResult? GetTurnRejection(Guid playerId, long expectedRevision)
    {
        MatchPlacementResult? rejection = null;
        if (Lifecycle.Status != MatchLifecycleStatus.Running)
        {
            rejection = MatchPlacementResult.MatchUnavailable;
        }
        else if (expectedRevision != _boardRevision)
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
                _hands[playerId] = cards.Where(card => card != move.CardNumber).ToArray();
                pile.TopCardNumber = move.CardNumber;
                pile.CardCount++;
                CompleteTurn(playerId, now);
                result = MatchPlacementResult.Applied;
            }
        }

        return result;
    }

    public MatchDismantleResult TryDismantle(Guid playerId, MatchDismantle move, DateTimeOffset now)
    {
        MatchDismantleResult result = GetTurnRejection(playerId, move.ExpectedRevision) switch
        {
            MatchPlacementResult.MatchUnavailable => MatchDismantleResult.MatchUnavailable,
            MatchPlacementResult.StaleRevision => MatchDismantleResult.StaleRevision,
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

        pile.TopCardNumber = _deck.RemainingCount > 0 ? _deck.Draw(1)[0] : null;
        pile.CardCount = pile.TopCardNumber.HasValue ? 1 : 0;
    }

    private void CompleteTurn(Guid playerId, DateTimeOffset now)
    {
        _boardRevision++;
        if (_hands[playerId].Count == 0)
        {
            CloseRound(playerId, now);
        }
        else if (_deck.RemainingCount == 0 && !Session.PlayerIds.Any(CanPlaceAnyModule))
        {
            CloseRound(null, now);
        }
        else
        {
            int index = Session.PlayerIds.ToList().IndexOf(playerId);
            _currentTurnPlayerId = Session.PlayerIds[(index + 1) % Session.PlayerIds.Count];
        }
    }

    private void CloseRound(Guid? closedBy, DateTimeOffset now)
    {
        _scoring.AddRound(_personalFloors, closedBy);
        if (_roundNumber == FinalRoundNumber)
        {
            _winnerIds = _scoring.GetWinners(Session.PlayerIds);
            _currentTurnPlayerId = Guid.Empty;
            TryTransition(MatchLifecycleStatus.Finished, now);
        }
        else
        {
            Guid opener = GetNextRoundOpener();
            _roundNumber++;
            _deck = _createDeck();
            DealRound(opener);
        }
    }

    private Guid GetNextRoundOpener()
    {
        IReadOnlyList<Guid> players = Session.PlayerIds;
        int previous = players.ToList().IndexOf(_roundOpenerId);
        IEnumerable<Guid> order = Enumerable.Range(1, players.Count)
            .Select(offset => players[(previous + offset) % players.Count]);

        return order.MinBy(_scoring.GetGlobalFloors);
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
                MasterPlans = _masterPlans[playerId]
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
        if (!IsTerminal && Session.PlayerIds.Contains(playerId))
        {
            changed = isConnected
                ? _disconnectedPlayerIds.Remove(playerId)
                : _disconnectedPlayerIds.Add(playerId);
            if (changed)
            {
                MatchLifecycleStatus status = _disconnectedPlayerIds.Count == Session.PlayerIds.Count
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
        if (!IsTerminal && Session.PlayerIds.Contains(playerId))
        {
            if (_disconnectedPlayerIds.Remove(playerId))
            {
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

    public bool IsTerminal => Lifecycle.Status is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled;

    public bool TryTransition(MatchLifecycleStatus next, DateTimeOffset changedAtUtc)
    {
        bool changed = false;
        if (!IsTerminal && (next is MatchLifecycleStatus.Finished or MatchLifecycleStatus.Cancelled))
        {
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
