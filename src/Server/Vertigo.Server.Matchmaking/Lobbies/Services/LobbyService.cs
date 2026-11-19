using Vertigo.Contracts.Internal;
using System.Security.Cryptography;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.Matchmaking.Lobbies;
using Vertigo.Server.Matchmaking.Lobbies.Match;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class LobbyService : ILobbyService, ILobbyDirectory
{
    private const string LobbyCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int LobbyCodeLength = 6;
    private const int MinimumMaxPlayers = 2;
    private const int MaximumMaxPlayers = 6;

    private readonly Dictionary<string, LobbyState> _lobbies = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lobbyLock = new();
    private readonly HashSet<string> _watchedSessions = new();
    private readonly TimeProvider _timeProvider;
    private readonly IFriendshipNotifier _notifier;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LobbyGameOperations _gameOperations;

    public LobbyService(TimeProvider timeProvider, IFriendshipNotifier notifier, IServiceScopeFactory scopeFactory,
        LobbyGameOperations gameOperations)
    {
        _timeProvider = timeProvider;
        _notifier = notifier;
        _scopeFactory = scopeFactory;
        _gameOperations = gameOperations;
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string sessionToken)
    {
        if (maxPlayers < MinimumMaxPlayers || maxPlayers > MaximumMaxPlayers)
        {
            throw CreateFault(ServiceErrorCodes.InvalidLobbyCapacity, ServiceMessageKeys.InvalidLobbyCapacity);
        }

        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember host = CreateMember(identity);
        LobbyConnection hostConnection = new(host, GetCurrentCallback(), WatchCurrentSession());
        DateTimeOffset createdAtUtc = _timeProvider.GetUtcNow();
        string lobbyCode;

        lock (_lobbyLock)
        {
            if (_lobbies.Values.Any(lobby => lobby.Contains(host.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.PlayerAlreadyInLobby, ServiceMessageKeys.PlayerAlreadyInLobby);
            }

            lobbyCode = GenerateUniqueLobbyCode();
            LobbySettings settings = new(lobbyCode, host.PlayerId, isPublic, maxPlayers, createdAtUtc);
            LobbyState lobby = new(settings);
            lobby.AddOrReplace(hostConnection);
            _lobbies.Add(lobbyCode, lobby);
        }

        return lobbyCode;
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        LobbyMember player = CreateMember(identity);
        LobbyConnection connection = new(player, GetCurrentCallback(), WatchCurrentSession());
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        List<ILobbyCallback> preparationCallbacks = new();
        LobbyPreparation? preparation = null;
        LobbyAdmission response;

        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            }

            if (lobby.Status != LobbyStatus.Waiting && !lobby.Contains(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
            }

            if (lobby.Contains(player.PlayerId))
            {
                if (lobby.Status == LobbyStatus.InGame)
                {
                    if (!_gameOperations.TryReconnect(lobby, player.PlayerId))
                    {
                        throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
                    }
                }
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
                response = CreateJoinResponse(lobby, player.PlayerId);
            }
            else if (lobby.IsFull)
            {
                throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
            }
            else if (!lobby.IsPublic && !lobby.IsInvited(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
            }
            else if (_lobbies.Values.Any(otherLobby => otherLobby.Contains(player.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.PlayerAlreadyInLobby, ServiceMessageKeys.PlayerAlreadyInLobby);
            }
            else
            {
                response = CreateJoinResponse(lobby, player.PlayerId);
                callbacksToNotify = lobby.GetCallbacksExcept(player.PlayerId);
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
            }
            preparationCallbacks = lobby.GetCallbacksExcept(Guid.Empty);
            preparation = lobby.ToPreparation();
        }

        Notify(callbacksToNotify, SendPlayerJoined, player);
        Notify(preparationCallbacks, SendPreparationChanged, preparation);

        return response;
    }

    // Leaving an active match is explicitly rejected without changing either module.
    // bool=false is used here because this method already returns bool.
    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        Guid playerId = identity.PlayerId;
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacksToNotify = new();
        List<Guid> withdrawnInvitedIds = new();
        LobbyPreparation? preparation = null;
        bool wasRemoved = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby)
                && lobby.Status == LobbyStatus.Waiting && lobby.Remove(playerId))
            {
                wasRemoved = true;
                if (lobby.IsEmpty)
                {
                    withdrawnInvitedIds = lobby.GetInvitedPlayerIds();
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else
                {
                    callbacksToNotify = lobby.GetCallbacksExcept(playerId);
                    preparation = lobby.ToPreparation();
                }
            }
        }

        Notify(callbacksToNotify, SendPlayerLeft, playerId);
        if (preparation is not null)
        {
            Notify(callbacksToNotify, SendPreparationChanged, preparation);
        }
        WithdrawInvitations(normalizedCode, withdrawnInvitedIds);
        return wasRemoved;
    }

    public LobbyPreparation GetPreparation(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            return lobby.ToPreparation();
        }
    }

    public LobbyPreparation? GetCurrentLobby(string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        lock (_lobbyLock)
        {
            LobbyState? lobby = _lobbies.Values.FirstOrDefault(candidate => candidate.Contains(identity.PlayerId));
            return lobby?.ToPreparation();
        }
    }

    public MatchPrivateHand GetMatchPrivateHand(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.InGame || lobby.MatchId == Guid.Empty)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyInGame, ServiceMessageKeys.LobbyAlreadyInGame);
            }

            return _gameOperations.GetPrivateHand(lobby.MatchId, identity.PlayerId)
                ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }
    }

    public MatchBoardSnapshot GetMatchBoard(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        MatchBoardSnapshot board;

        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.InGame || lobby.MatchId == Guid.Empty)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
            }

            board = _gameOperations.GetBoard(lobby)
                ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }

        return board;
    }

    public MatchBoardSnapshot PlaceMatchCard(MatchPlaceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SessionIdentity identity = ResolveIdentity(request.SessionToken);
        MatchBoardSnapshot board;
        List<ILobbyCallback> callbacks;
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(request.LobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.InGame || lobby.MatchId == Guid.Empty)
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotReady, ServiceMessageKeys.LobbyNotReady);
            }

            MatchPlacement move = new(request.CardNumber, request.PilePosition, request.ExpectedRevision);
            MatchPlacementResult result = _gameOperations.TryPlace(lobby.MatchId, identity.PlayerId, move);
            if (result != MatchPlacementResult.Applied)
            {
                throw LobbyPlacementFault.Create(result);
            }

            board = _gameOperations.GetBoard(lobby)
                ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);

            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        MatchBoardChanged changed = new()
        {
            LobbyCode = request.LobbyCode,
            MatchId = board.MatchId,
            Revision = board.Revision,
            PlayerUsername = identity.Username,
            CardNumber = request.CardNumber,
            PilePosition = request.PilePosition
        };
        Notify(callbacks, SendMatchBoardUpdated, changed);
        return board;
    }

    public LobbyPreparation SetReady(string lobbyCode, bool isReady, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        List<ILobbyCallback> callbacks;
        LobbyPreparation preparation;

        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.Waiting)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyInGame, ServiceMessageKeys.LobbyAlreadyInGame);
            }
            lobby.SetReady(identity.PlayerId, isReady);
            preparation = lobby.ToPreparation();
            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendPreparationChanged, preparation);
        return preparation;
    }

    public LobbyPreparation StartMatch(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        List<ILobbyCallback> callbacks;
        LobbyPreparation preparation;

        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(lobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.Waiting)
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyInGame, ServiceMessageKeys.LobbyAlreadyInGame);
            }
            if (lobby.HostPlayerId != identity.PlayerId)
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotHost, ServiceMessageKeys.LobbyNotHost);
            }
            if (!lobby.CanStartMatch(identity.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotReady, ServiceMessageKeys.LobbyNotReady);
            }

            // Entire transition is serialized with other lobby mutations.
            // No InGame state is committed if Game rejects initialization.
            _gameOperations.Activate(lobby);
            preparation = lobby.ToPreparation();
            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendPreparationChanged, preparation);
        return preparation;
    }

    // Callers already hold the lobby lock.
    private LobbyState RequireLobbyMember(string lobbyCode, Guid playerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        if (!_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }
        if (!lobby.Contains(playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotMember, ServiceMessageKeys.LobbyNotMember);
        }
        return lobby;
    }

    public List<LobbySummary> GetPublicLobbies()
    {
        List<LobbySummary> publicLobbies;

        lock (_lobbyLock)
        {
            publicLobbies = _lobbies.Values
                .Where(lobby => lobby.IsPublic && lobby.Status == LobbyStatus.Waiting && !lobby.IsFull)
                .OrderBy(lobby => lobby.CreatedAtUtc)
                .Select(lobby => lobby.ToSummary())
                .ToList();
        }

        return publicLobbies;
    }

    public LobbyOverview? FindLobby(string lobbyCode)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        LobbyOverview? overview = null;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                overview = lobby.ToOverview();
            }
        }

        return overview;
    }

    public bool IsInAnyLobby(Guid playerId)
    {
        bool isInLobby;

        lock (_lobbyLock)
        {
            isInLobby = _lobbies.Values.Any(lobby => lobby.Contains(playerId));
        }

        return isInLobby;
    }

    public bool RegisterInvitation(string lobbyCode, Guid targetPlayerId, Guid senderPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isRegistered = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby)
                && lobby.Status == LobbyStatus.Waiting
                && !lobby.Contains(targetPlayerId))
            {
                isRegistered = lobby.Invite(targetPlayerId, senderPlayerId);
            }
        }

        return isRegistered;
    }

    public bool CancelInvitation(string lobbyCode, Guid targetPlayerId)
    {
        string normalizedCode = lobbyCode?.Trim() ?? string.Empty;
        bool isCancelled = false;

        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(normalizedCode, out LobbyState? lobby))
            {
                isCancelled = lobby.CancelInvitation(targetPlayerId);
            }
        }

        return isCancelled;
    }

    public List<LobbyOverview> FindLobbiesInviting(Guid playerId)
    {
        List<LobbyOverview> overviews;

        lock (_lobbyLock)
        {
            overviews = _lobbies.Values
                .Where(lobby => lobby.Status == LobbyStatus.Waiting && lobby.IsInvited(playerId))
                .Select(lobby => lobby.ToOverview())
                .ToList();
        }

        return overviews;
    }

    // A closed or faulted channel means the client left or dropped: its players are removed from every lobby.
    public bool PublishChatMessage(LobbyChatMessage message)
    {
        List<ILobbyCallback> callbacks;
        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(message.LobbyCode, out LobbyState? lobby)
                || lobby.Status != LobbyStatus.Waiting
                || !lobby.Contains(message.SenderId))
            {
                return false;
            }

            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        Notify(callbacks, SendChatMessage, message);
        return true;
    }

    private SessionIdentity ResolveIdentity(string sessionToken)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        return resolver.Require(sessionToken);
    }

    private static LobbyMember CreateMember(SessionIdentity identity)
    {
        return new LobbyMember
        {
            PlayerId = identity.PlayerId,
            Username = identity.Username,
            IsGuest = identity.IsGuest
        };
    }

    private static void SendPreparationChanged(ILobbyCallback callback, LobbyPreparation preparation)
    {
        callback.PreparationChanged(preparation);
    }

    private static void SendMatchBoardUpdated(ILobbyCallback callback, MatchBoardChanged change)
    {
        callback.MatchBoardUpdated(change);
    }

    private static void SendChatMessage(ILobbyCallback callback, LobbyChatMessage message)
    {
        callback.MessageReceived(message);
    }

    private string WatchCurrentSession()
    {
        IContextChannel channel = (IContextChannel)GetCurrentCallback();
        string sessionId = OperationContext.Current.SessionId ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw CreateFault(ServiceErrorCodes.Unauthorized, ServiceMessageKeys.Unauthorized);
        }

        bool isNewSession;

        lock (_lobbyLock)
        {
            isNewSession = _watchedSessions.Add(sessionId);
        }

        if (isNewSession)
        {
            channel.Closed += OnChannelClosed;
            channel.Faulted += OnChannelClosed;
        }

        return sessionId;
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        string sessionId = (sender as IContextChannel)?.SessionId ?? string.Empty;
        List<(List<ILobbyCallback> Callbacks, List<Guid> LeftIds, LobbyPreparation State)> notifications = new();
        List<(string LobbyCode, List<Guid> InvitedIds)> withdrawals = new();

        lock (_lobbyLock)
        {
            _watchedSessions.Remove(sessionId);

            foreach (LobbyState lobby in _lobbies.Values.ToList())
            {
                List<Guid> leftIds;
                if (lobby.Status == LobbyStatus.InGame)
                {
                    leftIds = lobby.DisconnectSession(sessionId);
                    foreach (Guid playerId in leftIds)
                    {
                        _gameOperations.Disconnect(lobby, playerId);
                    }
                }
                else
                {
                    leftIds = lobby.RemoveSession(sessionId);
                }

                if (lobby.IsEmpty)
                {
                    withdrawals.Add((lobby.LobbyCode, lobby.GetInvitedPlayerIds()));
                    _lobbies.Remove(lobby.LobbyCode);
                }
                else if (leftIds.Count > 0)
                {
                    notifications.Add((lobby.GetCallbacksExcept(Guid.Empty), leftIds, lobby.ToPreparation()));
                }
            }
        }

        foreach ((List<ILobbyCallback> callbacks, List<Guid> leftIds, LobbyPreparation state) in notifications)
        {
            // During a match, disconnected players retain their participant identity.
            // PlayerLeft is reserved for actual removals during the waiting phase.
            if (!state.IsInGame)
            {
                foreach (Guid leftId in leftIds)
                {
                    Notify(callbacks, SendPlayerLeft, leftId);
                }
            }
            Notify(callbacks, SendPreparationChanged, state);
        }

        foreach ((string lobbyCode, List<Guid> invitedIds) in withdrawals)
        {
            WithdrawInvitations(lobbyCode, invitedIds);
        }
    }

    private void WithdrawInvitations(string lobbyCode, List<Guid> invitedIds)
    {
        foreach (Guid invitedId in invitedIds)
        {
            _notifier.NotifyLobbyInvitationWithdrawn(invitedId, lobbyCode);
        }
    }

    private string GenerateUniqueLobbyCode()
    {
        string lobbyCode;

        do
        {
            lobbyCode = RandomNumberGenerator.GetString(LobbyCodeAlphabet, LobbyCodeLength);
        }
        while (_lobbies.ContainsKey(lobbyCode));

        return lobbyCode;
    }

    private static LobbyAdmission CreateJoinResponse(LobbyState lobby, Guid joiningPlayerId)
    {
        LobbyAdmission response = new()
        {
            LobbyCode = lobby.LobbyCode,
            HostPlayerId = lobby.HostPlayerId,
            IsPublic = lobby.IsPublic,
            MaxPlayers = lobby.MaxPlayers,
            ExistingMembers = lobby.GetMembersExcept(joiningPlayerId)
        };

        return response;
    }

    private static ILobbyCallback GetCurrentCallback()
    {
        ILobbyCallback callback = OperationContext.Current.GetCallbackChannel<ILobbyCallback>();

        return callback;
    }

    private static void SendPlayerJoined(ILobbyCallback callback, LobbyMember player)
    {
        callback.PlayerJoined(player);
    }

    private static void SendPlayerLeft(ILobbyCallback callback, Guid playerId)
    {
        callback.PlayerLeft(playerId);
    }

    private static void Notify<TArgument>(
        IReadOnlyList<ILobbyCallback> callbacks,
        Action<ILobbyCallback, TArgument> notification,
        TArgument argument)
    {
        foreach (ILobbyCallback callback in callbacks)
        {
            try
            {
                notification(callback, argument);
            }
            catch (CommunicationException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (TimeoutException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
            catch (ObjectDisposedException exception)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + exception.Message);
            }
        }
    }

    private static FaultException<ServiceFault> CreateFault(string errorCode, string messageKey)
    {
        ServiceFault fault = new()
        {
            ErrorCode = errorCode,
            MessageKey = messageKey
        };

        return new FaultException<ServiceFault>(fault, new FaultReason(errorCode));
    }
}
