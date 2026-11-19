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
    private readonly LobbyLifecycle _lifecycle;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LobbyGameOperations _gameOperations;

    public LobbyService(LobbyLifecycle lifecycle, IServiceScopeFactory scopeFactory,
        LobbyGameOperations gameOperations)
    {
        _lifecycle = lifecycle;
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
        LobbyCreation creation = _lifecycle.PrepareCreation(isPublic, maxPlayers);
        string lobbyCode;

        lock (_lobbyLock)
        {
            if (_lobbies.Values.Any(lobby => lobby.Contains(host.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.PlayerAlreadyInLobby, ServiceMessageKeys.PlayerAlreadyInLobby);
            }

            lobbyCode = GenerateUniqueLobbyCode();
            LobbyState lobby = _lifecycle.CreateLobby(lobbyCode, hostConnection, creation);
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
        LobbyPreparation preparation;
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

        Notify(callbacksToNotify, CreatePlayerJoinedNotification(player));
        Notify(preparationCallbacks, CreatePreparationNotification(preparation));

        return response;
    }

    // LeaveLobby is for the waiting room. Explicit match departure uses LeaveMatch.
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

        Notify(callbacksToNotify, CreatePlayerLeftNotification(playerId));
        if (preparation is not null)
        {
            Notify(callbacksToNotify, CreatePreparationNotification(preparation));
        }
        _lifecycle.WithdrawInvitations(normalizedCode, withdrawnInvitedIds);
        return wasRemoved;
    }

    public bool LeaveMatch(MatchLeaveRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        SessionIdentity identity = ResolveIdentity(request.SessionToken);
        string lobbyCode = request.LobbyCode?.Trim() ?? string.Empty;
        List<ILobbyCallback> callbacks = new();
        List<Guid> withdrawnInvitedIds = new();
        MatchBoardChanged? notice = null;
        bool left = false;

        lock (_lobbyLock)
        {
            if (!_lobbies.TryGetValue(lobbyCode, out LobbyState? lobby) || !lobby.Contains(identity.PlayerId))
            {
                left = true;
            }
            else if (lobby.Status == LobbyStatus.InGame && request.MatchId == lobby.MatchId)
            {
                MatchDepartureResult departure = _gameOperations.TryLeaveMatch(lobby.MatchId, identity.PlayerId);
                if (departure.IsAccepted)
                {
                    left = lobby.Remove(identity.PlayerId);
                    if (lobby.IsEmpty)
                    {
                        withdrawnInvitedIds = lobby.GetInvitedPlayerIds();
                        _lobbies.Remove(lobby.LobbyCode);
                    }
                    else if (departure.BoardChanged)
                    {
                        callbacks = lobby.GetCallbacksExcept(Guid.Empty);
                        notice = CreateDepartureNotice(lobby);
                    }
                }
            }
        }

        if (notice is not null)
        {
            Notify(callbacks, CreateBoardNotification(notice));
        }

        _lifecycle.WithdrawInvitations(lobbyCode, withdrawnInvitedIds);
        return left;
    }

    private MatchBoardChanged? CreateDepartureNotice(LobbyState lobby)
    {
        MatchBoardSnapshot? board = _gameOperations.GetBoard(lobby);
        MatchBoardMove? lastMove = board?.RecentMoves.LastOrDefault();
        return board is not null && lastMove is not null
            ? new MatchBoardChanged
            {
                LobbyCode = lobby.LobbyCode,
                MatchId = board.MatchId,
                Revision = board.Revision,
                PlayerUsername = lastMove.PlayerUsername,
                ActionKind = lastMove.ActionKind
            }
            : null;
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

            MatchPrivateHand? hand = _gameOperations.GetPrivateHand(lobby.MatchId, identity.PlayerId);
            if (hand is null)
            {
                MatchBoardSnapshot? board = _gameOperations.GetBoard(lobby);
                if (board is not null && board.ExpelledPlayerIds.Contains(identity.PlayerId))
                {
                    hand = new MatchPrivateHand
                    {
                        MatchId = lobby.MatchId,
                        PlayerId = identity.PlayerId,
                        Revision = board.Revision
                    };
                }
            }

            return hand ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
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
        MatchPlacement move = new(request.CardNumber, request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken),
            (move.PilePosition, move.CardNumber), ApplyPlacement);

        MatchBoardActionKind ApplyPlacement(Guid matchId, Guid playerId)
        {
            MatchPlacementResult result = _gameOperations.TryPlace(matchId, playerId, move);
            if (result != MatchPlacementResult.Applied)
            {
                throw LobbyMatchFault.Create(result);
            }

            return MatchBoardActionKind.Placement;
        }
    }

    public MatchBoardSnapshot DismantleMatchTower(MatchDismantleRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        MatchDismantle move = new(request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken),
            (move.PilePosition, 0), ApplyDismantle);

        MatchBoardActionKind ApplyDismantle(Guid matchId, Guid playerId)
        {
            MatchDismantleResult result = _gameOperations.TryDismantle(matchId, playerId, move);
            if (result != MatchDismantleResult.Applied)
            {
                throw LobbyMatchFault.Create(result);
            }

            return MatchBoardActionKind.Dismantle;
        }
    }

    public MatchBoardSnapshot UseMatchMasterPlan(MatchMasterPlanRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        int[] modules = request.CardNumbers?.ToArray() ?? Array.Empty<int>();
        MatchMasterPlan move = new(modules, request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken),
            (move.PilePosition, modules.LastOrDefault()), ApplyMasterPlan);

        MatchBoardActionKind ApplyMasterPlan(Guid matchId, Guid playerId)
        {
            MatchPlacementResult result = _gameOperations.TryUseMasterPlan(matchId, playerId, move);
            if (result != MatchPlacementResult.Applied)
            {
                throw LobbyMatchFault.Create(result);
            }

            return MatchBoardActionKind.MasterPlan;
        }
    }

    public MatchBoardSnapshot VoteToExpelMatchPlayer(MatchExpulsionVoteRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        Guid matchToVote = request.MatchId;
        MatchExpulsionVote vote = new(request.TargetPlayerId, request.ExpectedVotingRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken), (0, 0), ApplyVote);

        MatchBoardActionKind ApplyVote(Guid matchId, Guid voterId)
        {
            MatchExpulsionVoteResult result = matchId == matchToVote
                ? _gameOperations.TryVoteToExpel(matchId, voterId, vote)
                : MatchExpulsionVoteResult.MatchUnavailable;

            return result switch
            {
                MatchExpulsionVoteResult.Recorded => MatchBoardActionKind.ExpulsionVoteRecorded,
                MatchExpulsionVoteResult.Expelled => MatchBoardActionKind.ExpelledByVote,
                _ => throw LobbyMatchFault.Create(result)
            };
        }
    }

    private MatchBoardSnapshot ApplyMatchBoardAction(
        (string LobbyCode, string SessionToken) access,
        (int PilePosition, int CardNumber) action,
        Func<Guid, Guid, MatchBoardActionKind> apply)
    {
        SessionIdentity identity = ResolveIdentity(access.SessionToken);
        MatchBoardSnapshot board;
        MatchBoardActionKind kind;
        List<ILobbyCallback> callbacks;
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(access.LobbyCode, identity.PlayerId);
            if (lobby.Status != LobbyStatus.InGame || lobby.MatchId == Guid.Empty)
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotReady, ServiceMessageKeys.LobbyNotReady);
            }

            kind = apply(lobby.MatchId, identity.PlayerId);
            board = _gameOperations.GetBoard(lobby)
                ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            callbacks = lobby.GetCallbacksExcept(Guid.Empty);
        }

        MatchBoardChanged changed = new()
        {
            LobbyCode = access.LobbyCode,
            MatchId = board.MatchId,
            Revision = board.Revision,
            PlayerUsername = kind switch
            {
                MatchBoardActionKind.ExpelledByVote => board.RecentMoves.Last().PlayerUsername,
                MatchBoardActionKind.ExpulsionVoteRecorded => string.Empty,
                _ => identity.Username
            },
            ActionKind = kind,
            CardNumber = action.CardNumber,
            PilePosition = action.PilePosition
        };
        Notify(callbacks, CreateBoardNotification(changed));
        return board;
    }

    public void ProcessExpiredMatchTurns()
    {
        List<(List<ILobbyCallback> Callbacks, MatchBoardChanged Notice)> notifications = new();
        lock (_lobbyLock)
        {
            foreach (LobbyState lobby in _lobbies.Values)
            {
                if (lobby.Status == LobbyStatus.InGame
                    && _gameOperations.TryExpireTurn(lobby.MatchId) is Guid expelledId)
                {
                    MatchBoardSnapshot? board = _gameOperations.GetBoard(lobby);
                    MatchBoardMove? move = board?.RecentMoves.LastOrDefault();
                    if (board is not null && move is not null && move.PlayerId == expelledId)
                    {
                        MatchBoardChanged notice = new()
                        {
                            LobbyCode = lobby.LobbyCode,
                            MatchId = board.MatchId,
                            Revision = board.Revision,
                            PlayerUsername = move.PlayerUsername,
                            ActionKind = MatchBoardActionKind.TurnExpired
                        };
                        notifications.Add((lobby.GetCallbacksExcept(Guid.Empty), notice));
                    }
                }
            }
        }

        foreach ((List<ILobbyCallback> callbacks, MatchBoardChanged notice) in notifications)
        {
            Notify(callbacks, CreateBoardNotification(notice));
        }
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

        Notify(callbacks, CreatePreparationNotification(preparation));
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

        Notify(callbacks, CreatePreparationNotification(preparation));
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

        Notify(callbacks, CreateChatNotification(message));
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

    private static LobbyNotification CreatePreparationNotification(LobbyPreparation preparation)
    {
        return new LobbyNotification
        {
            Kind = LobbyNotificationKind.PreparationChanged,
            Preparation = preparation
        };
    }

    private static LobbyNotification CreateBoardNotification(MatchBoardChanged change)
    {
        return new LobbyNotification
        {
            Kind = LobbyNotificationKind.MatchBoardUpdated,
            BoardChange = change
        };
    }

    private static LobbyNotification CreateChatNotification(LobbyChatMessage message)
    {
        return new LobbyNotification
        {
            Kind = LobbyNotificationKind.MessageReceived,
            ChatMessage = message
        };
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
                    Notify(callbacks, CreatePlayerLeftNotification(leftId));
                }
            }
            Notify(callbacks, CreatePreparationNotification(state));
        }

        foreach ((string lobbyCode, List<Guid> invitedIds) in withdrawals)
        {
            _lifecycle.WithdrawInvitations(lobbyCode, invitedIds);
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

    private static LobbyNotification CreatePlayerJoinedNotification(LobbyMember player)
    {
        return new LobbyNotification
        {
            Kind = LobbyNotificationKind.PlayerJoined,
            JoinedPlayer = player
        };
    }

    private static LobbyNotification CreatePlayerLeftNotification(Guid playerId)
    {
        return new LobbyNotification
        {
            Kind = LobbyNotificationKind.PlayerLeft,
            LeftPlayerId = playerId
        };
    }

    private static void Notify(IReadOnlyList<ILobbyCallback> callbacks, LobbyNotification notification)
    {
        foreach (ILobbyCallback callback in callbacks)
        {
            try
            {
                callback.Notify(notification);
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
