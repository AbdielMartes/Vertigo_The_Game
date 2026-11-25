using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;

namespace Vertigo.Server.Matchmaking.Lobbies;

public sealed class LobbyState
{
    internal const int MinimumCapacity = 2;
    internal const int MaximumCapacity = 6;

    private readonly List<LobbyConnection> _connections = new();
    private readonly Dictionary<Guid, Guid> _invitations = new();
    private readonly HashSet<Guid> _readyPlayerIds = new();
    private readonly HashSet<Guid> _disconnectedPlayerIds = new();
    private long _revision;
    private long _chatSequence;
    private Guid _matchId;

    public Guid LobbyInstanceId { get; }

    public Guid MatchId => _matchId;

    public IReadOnlyDictionary<Guid, LobbyMember> MatchPlayers { get; private set; } =
        new Dictionary<Guid, LobbyMember>();

    public long Revision => _revision;

    internal object ChatDispatchLock { get; } = new();

    internal long NextChatSequence()
    {
        return ++_chatSequence;
    }

    public LobbyState(LobbySettings settings, Guid? lobbyInstanceId = null)
    {
        LobbyInstanceId = lobbyInstanceId ?? Guid.NewGuid();
        ArgumentNullException.ThrowIfNull(settings);
        if (!IsValidCapacity(settings.MaxPlayers))
        {
            throw new ArgumentOutOfRangeException(nameof(settings.MaxPlayers));
        }

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

    internal static bool IsValidCapacity(int capacity)
    {
        return capacity >= MinimumCapacity && capacity <= MaximumCapacity;
    }

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
            _readyPlayerIds.Clear();
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

        if (removedCount > 0 && playerId == HostPlayerId && _connections.Count > 0)
        {
            HostPlayerId = _connections[0].Member.PlayerId;
        }

        return removedCount > 0;
    }

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
        LobbyOverview overview = new()
        {
            LobbyCode = LobbyCode,
            HostPlayerId = HostPlayerId,
            MaxPlayers = MaxPlayers,
            IsWaiting = Status == LobbyStatus.Waiting,
            Members = GetMembersExcept(Guid.Empty),
            Invitations = new Dictionary<Guid, Guid>(_invitations)
        };

        return overview;
    }

    public bool SetReady(Guid playerId, bool isReady)
    {
        bool canSetReady = Status == LobbyStatus.Waiting && Contains(playerId);
        if (canSetReady)
        {
            bool changed = isReady ? _readyPlayerIds.Add(playerId) : _readyPlayerIds.Remove(playerId);
            if (changed)
            {
                _revision++;
            }
        }

        return canSetReady;
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

        MatchPlayers = _connections.ToDictionary(connection => connection.Member.PlayerId,
            connection => connection.Member);
        _matchId = matchId;
        Status = LobbyStatus.InGame;
        _revision++;
    }

    internal void RestoreMatch(Guid matchId, RecoverableLobby lobby, IReadOnlyList<Guid> activePlayers)
    {
        foreach (LobbyMember member in lobby.Members.Where(member => activePlayers.Contains(member.PlayerId)))
        {
            AddOrReplace(new LobbyConnection(member, null, string.Empty));
            _disconnectedPlayerIds.Add(member.PlayerId);
        }

        StartMatch(matchId);
        MatchPlayers = lobby.Members.ToDictionary(member => member.PlayerId);
    }

    public List<LobbyMember> GetMembersExcept(Guid playerId)
    {
        List<LobbyMember> members = _connections
            .Where(connection => connection.Member.PlayerId != playerId)
            .Select(connection => connection.Member)
            .ToList();

        return members;
    }

    public bool IsDisconnected(Guid playerId) => _disconnectedPlayerIds.Contains(playerId);

    public List<ILobbyCallback> GetCallbacksExcept(Guid playerId)
    {
        return GetConnectedCallbacks(id => id != playerId);
    }

    public List<ILobbyCallback> GetConnectedCallbacks(Predicate<Guid> includesPlayer)
    {
        return _connections
            .Where(connection => includesPlayer(connection.Member.PlayerId)
                && !_disconnectedPlayerIds.Contains(connection.Member.PlayerId))
            .Select(connection => connection.Callback)
            .OfType<ILobbyCallback>()
            .ToList();
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
