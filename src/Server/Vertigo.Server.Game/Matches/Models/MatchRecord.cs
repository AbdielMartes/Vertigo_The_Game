using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Game.Matches.Models;

// Protected by the owning starter's synchronization lock.
internal sealed class MatchRecord
{
    // Provisional board layout; confirm the final pile count against Vértigo's rules.
    private const int InitialDiscardPileCount = 4;
    private const int FirstRoundNumber = 1;
    private const long InitialBoardRevision = 1;

    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private readonly Dictionary<Guid, IReadOnlyList<int>> _hands = new();
    private readonly MatchDeck _deck = new();
    private readonly List<MatchBoardPile> _piles;
    private readonly int _roundNumber;
    private Guid _currentTurnPlayerId;
    private long _boardRevision;

    public MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc)
    {
        Session = session;
        // Four distinct foundation modules are drawn before private hands.
        IReadOnlyList<int> foundations = _deck.Draw(InitialDiscardPileCount);
        foreach (Guid playerId in session.PlayerIds)
        {
            _hands.Add(playerId, _deck.Draw(MatchDeck.InitialHandSize));
        }
        _roundNumber = FirstRoundNumber;
        // Host-first is provisional until the first-player rule is confirmed.
        _currentTurnPlayerId = session.InitialHostPlayerId;
        _boardRevision = InitialBoardRevision;
        _piles = Enumerable.Range(1, InitialDiscardPileCount)
            .Select(position => new MatchBoardPile
            {
                Position = position,
                TopCardNumber = foundations[position - 1],
                CardCount = 1
            }).ToList();
        Lifecycle = new MatchLifecycleSnapshot
        {
            MatchId = session.MatchId,
            LobbyInstanceId = session.LobbyInstanceId,
            Status = MatchLifecycleStatus.Running,
            Revision = 1,
            UpdatedAtUtc = createdAtUtc
        };
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
            Players = Session.PlayerIds.Select(id => new MatchBoardPlayer
            {
                PlayerId = id,
                FloorsBuilt = 0
            }).ToList(),
            Piles = _piles.Select(pile => new MatchBoardPile
            {
                Position = pile.Position,
                TopCardNumber = pile.TopCardNumber,
                CardCount = pile.CardCount
            }).ToList()
        };
    }

    public MatchPlacementResult TryPlace(Guid playerId, MatchPlacement move)
    {
        MatchPlacementResult result;
        if (IsTerminal || Lifecycle.Status != MatchLifecycleStatus.Running)
        {
            result = MatchPlacementResult.MatchUnavailable;
        }
        else if (move.ExpectedRevision != _boardRevision)
        {
            result = MatchPlacementResult.StaleRevision;
        }
        else if (playerId != _currentTurnPlayerId)
        {
            result = MatchPlacementResult.NotYourTurn;
        }
        else if (!_hands.TryGetValue(playerId, out IReadOnlyList<int>? cards)
                 || !cards.Contains(move.CardNumber))
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
                _boardRevision++;
                int index = Session.PlayerIds.ToList().IndexOf(playerId);
                _currentTurnPlayerId = Session.PlayerIds[(index + 1) % Session.PlayerIds.Count];
                result = MatchPlacementResult.Applied;
            }
        }

        return result;
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
                Cards = cards.ToList()
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
