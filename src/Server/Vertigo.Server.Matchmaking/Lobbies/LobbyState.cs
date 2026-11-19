using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyState
{
    private readonly List<LobbyConnection> _connections = new();
    private readonly Dictionary<Guid, Guid> _invitations = new();
    private readonly HashSet<Guid> _readyPlayerIds = new();
    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private long _revision;
    private Guid _matchId;

    public Guid LobbyInstanceId { get; } = Guid.NewGuid();

    public Guid MatchId => _matchId;

    public IReadOnlyDictionary<Guid, string> MatchPlayerNames { get; private set; } =
        new Dictionary<Guid, string>();

    public long Revision => _revision;

    public LobbyState(LobbySettings settings)
    {
        LobbyCode = settings.LobbyCode;
        HostPlayerId = settings.HostPlayerId;
        IsPublic = settings.IsPublic;
        MaxPlayers = settings.MaxPlayers;
        CreatedAtUtc = settings.CreatedAtUtc;
        Status = LobbyStatus.Waiting;
    }

    public string LobbyCode { get; }

    public Guid HostPlayerId { get; private set; }

    public bool IsPublic { get; }

    public int MaxPlayers { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public LobbyStatus Status { get; private set; }

    public bool IsEmpty => _connections.Count == 0;

    public bool IsFull => _connections.Count >= MaxPlayers;

    public bool Contains(Guid playerId)
    {
        bool contains = _connections.Exists(connection => connection.Member.PlayerId == playerId);

        return contains;
    }

    public void AddOrReplace(LobbyConnection connection)
    {
        int index = _connections.FindIndex(existing => existing.Member.PlayerId == connection.Member.PlayerId);
        if (index >= 0)
        {
            _connections[index] = connection;
            _disconnectedPlayerIds.Remove(connection.Member.PlayerId);
            _revision++;
        }
        else
        {
            _connections.Add(connection);
            _readyPlayerIds.Clear(); // Joining changes the preparation quorum.
            _revision++;
        }
    }

    public bool Remove(Guid playerId)
    {
        int removedCount = _connections.RemoveAll(connection => connection.Member.PlayerId == playerId);
        if (removedCount > 0)
        {
            _readyPlayerIds.Remove(playerId);
            _disconnectedPlayerIds.Remove(playerId);
            _revision++;
        }

        // The role passes to the participant who has been longest in the lobby (CU-014 FA-04).
        if (removedCount > 0 && playerId == HostPlayerId && _connections.Count > 0)
        {
            HostPlayerId = _connections[0].Member.PlayerId;
        }

        return removedCount > 0;
    }

    // In-game transport loss does not remove a player or transfer the host role.
    // A delayed Closed event from the old channel cannot disconnect a new channel.
    public List<Guid> DisconnectSession(string sessionId)
    {
        List<Guid> changed = new();
        foreach (LobbyConnection connection in _connections)
        {
            if (connection.SessionId == sessionId
                && _disconnectedPlayerIds.Add(connection.Member.PlayerId))
            {
                changed.Add(connection.Member.PlayerId);
                _revision++;
            }
        }

        return changed;
    }

    public List<Guid> RemoveSession(string sessionId)
    {
        List<Guid> removedIds = _connections
            .Where(connection => connection.SessionId == sessionId)
            .Select(connection => connection.Member.PlayerId)
            .ToList();

        foreach (Guid playerId in removedIds)
        {
            Remove(playerId);
        }

        return removedIds;
    }

    public bool Invite(Guid targetPlayerId, Guid senderPlayerId)
    {
        bool isInvited = _invitations.TryAdd(targetPlayerId, senderPlayerId);

        return isInvited;
    }

    public bool CancelInvitation(Guid targetPlayerId)
    {
        bool isCancelled = _invitations.Remove(targetPlayerId);

        return isCancelled;
    }

    public bool IsInvited(Guid playerId)
    {
        bool isInvited = _invitations.ContainsKey(playerId);

        return isInvited;
    }

    public List<Guid> GetInvitedPlayerIds()
    {
        List<Guid> invitedIds = _invitations.Keys.ToList();

        return invitedIds;
    }

    public LobbyOverview ToOverview()
    {
        LobbyOverview overview = new(
            LobbyCode,
            HostPlayerId,
            MaxPlayers,
            Status == LobbyStatus.Waiting,
            GetMembersExcept(Guid.Empty),
            new Dictionary<Guid, Guid>(_invitations));

        return overview;
    }

    public bool SetReady(Guid playerId, bool isReady)
    {
        if (Status != LobbyStatus.Waiting || !Contains(playerId))
        {
            return false;
        }

        bool changed = isReady ? _readyPlayerIds.Add(playerId) : _readyPlayerIds.Remove(playerId);
        if (changed)
        {
            _revision++;
        }

        return true;
    }

    public bool CanStartMatch(Guid requesterId)
    {
        return Status == LobbyStatus.Waiting
            && requesterId == HostPlayerId
            && _connections.Count >= 2
            && _connections.All(connection => _readyPlayerIds.Contains(connection.Member.PlayerId));
    }

    public LobbyPreparation ToPreparation()
    {
        return new LobbyPreparation
        {
            LobbyCode = LobbyCode,
            Revision = _revision,
            HostPlayerId = HostPlayerId,
            IsInGame = Status == LobbyStatus.InGame,
            IsSuspended = Status == LobbyStatus.InGame && _connections.Count > 0
                && _disconnectedPlayerIds.Count == _connections.Count,
            DisconnectedPlayerIds = _disconnectedPlayerIds.OrderBy(id => id).ToList(),
            MatchId = _matchId,
            ReadyPlayerIds = _readyPlayerIds.ToList()
        };
    }

    public void StartMatch(Guid matchId)
    {
        if (matchId == Guid.Empty)
        {
            throw new ArgumentException("A created match identifier is required.", nameof(matchId));
        }

        if (Status != LobbyStatus.Waiting || _matchId != Guid.Empty)
        {
            throw new InvalidOperationException("The lobby has already started a match.");
        }

        MatchPlayerNames = _connections.ToDictionary(connection => connection.Member.PlayerId,
            connection => connection.Member.Username);
        _matchId = matchId;
        Status = LobbyStatus.InGame;
        _revision++;
    }

    public List<LobbyMember> GetMembersExcept(Guid playerId)
    {
        List<LobbyMember> members = _connections
            .Where(connection => connection.Member.PlayerId != playerId)
            .Select(connection => connection.Member)
            .ToList();

        return members;
    }

    public List<ILobbyCallback> GetCallbacksExcept(Guid playerId)
    {
        List<ILobbyCallback> callbacks = _connections
            .Where(connection => connection.Member.PlayerId != playerId
                && !_disconnectedPlayerIds.Contains(connection.Member.PlayerId))
            .Select(connection => connection.Callback)
            .ToList();

        return callbacks;
    }

    public LobbySummary ToSummary()
    {
        LobbyConnection? hostConnection = _connections.Find(
            connection => connection.Member.PlayerId == HostPlayerId);

        LobbySummary summary = new()
        {
            LobbyCode = LobbyCode,
            HostUsername = hostConnection?.Member.Username ?? string.Empty,
            CurrentPlayers = _connections.Count,
            MaxPlayers = MaxPlayers
        };

        return summary;
    }
}
