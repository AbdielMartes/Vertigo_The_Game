using Vertigo.Contracts.Internal;

namespace Vertigo.Server.Game.Matches.Models;

// Protected by the owning starter's synchronization lock.
internal sealed class MatchRecord
{
    public MatchRecord(MatchSessionSnapshot session, DateTimeOffset createdAtUtc)
    {
        Session = session;
        Lifecycle = new MatchLifecycleSnapshot
        {
            MatchId = session.MatchId,
            LobbyInstanceId = session.LobbyInstanceId,
            Status = MatchLifecycleStatus.Running,
            Revision = 1,
            UpdatedAtUtc = createdAtUtc
        };
    }

    private readonly HashSet<Guid> _disconnectedPlayerIds = new();

    public MatchSessionSnapshot Session { get; }

    public MatchLifecycleSnapshot Lifecycle { get; private set; }

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
