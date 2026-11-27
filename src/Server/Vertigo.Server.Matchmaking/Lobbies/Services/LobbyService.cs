using Vertigo.Contracts.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using CoreWCF;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Server.Matchmaking.Lobbies;
using Vertigo.Server.Matchmaking.Lobbies.Match;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Lobbies.Services;

[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
public sealed class LobbyService : ILobbyService, ILobbyDirectory
{
    private const string LobbyCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int LobbyCodeLength = 6;
    private static readonly EventId InvalidSessionHeaderEvent = new(2104, "InvalidLobbySessionHeader");

    private readonly Dictionary<string, LobbyState> _lobbies = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<Guid> _turnClockWriteFailures = new();
    private readonly object _lobbyLock = new();
    private readonly HashSet<string> _watchedSessions = new();
    private readonly LobbyLifecycle _lifecycle;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly LobbyGameOperations _gameOperations;

    public LobbyService(LobbyLifecycle lifecycle, IServiceScopeFactory scopeFactory,
        LobbyGameOperations gameOperations)
    {
        ArgumentNullException.ThrowIfNull(lifecycle);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(gameOperations);
        _lifecycle = lifecycle;
        _scopeFactory = scopeFactory;
        _gameOperations = gameOperations;
        using IServiceScope scope = scopeFactory.CreateScope();
        IMatchRecovery? recovery = scope.ServiceProvider.GetService<IMatchRecovery>();
        if (recovery is not null)
        {
            RestoreLobbies(recovery);
        }
    }

    private void RestoreLobbies(IMatchRecovery recovery)
    {
        foreach ((MatchSessionSnapshot session, RecoverableLobby data, IReadOnlyList<Guid> players)
            in recovery.GetRecoveredMatches())
        {
            LobbySettings settings = new()
            {
                LobbyCode = session.LobbyCode,
                HostPlayerId = players.Contains(session.InitialHostPlayerId)
                    ? session.InitialHostPlayerId : players[0],
                IsPublic = data.IsPublic,
                MaxPlayers = data.MaxPlayers,
                CreatedAtUtc = data.CreatedAtUtc
            };
            LobbyState lobby = new(settings, session.LobbyInstanceId);
            lobby.RestoreMatch(session.MatchId, data, players);
            _lobbies.Add(lobby.LobbyCode, lobby);
        }
    }

    public string CreateLobby(bool isPublic, int maxPlayers, string? sessionToken)
    {
        if (!LobbyState.IsValidCapacity(maxPlayers))
        {
            throw CreateFault(ServiceErrorCodes.InvalidLobbyCapacity, ServiceMessageKeys.InvalidLobbyCapacity);
        }

        SessionIdentity identity = ResolveIdentity(ReadCreationToken(sessionToken));
        LobbyMember host = CreateMember(identity);
        LobbyCreation creation = _lifecycle.PrepareCreation(isPublic, maxPlayers);
        string lobbyCode;

        lock (_lobbyLock)
        {
            if (_lobbies.Values.Any(lobby => lobby.Contains(host.PlayerId)))
            {
                throw CreateFault(ServiceErrorCodes.PlayerAlreadyInLobby, ServiceMessageKeys.PlayerAlreadyInLobby);
            }

            LobbyConnection hostConnection = new(host, GetCurrentCallback(), WatchCurrentSession());
            lobbyCode = GenerateUniqueLobbyCode();
            LobbyState lobby = _lifecycle.CreateLobby(lobbyCode, hostConnection, creation);
            _lobbies.Add(lobbyCode, lobby);
        }

        return lobbyCode;
    }

    public LobbyAdmission JoinLobby(string lobbyCode, string sessionToken)
    {
        return JoinLobby((lobbyCode, sessionToken), null);
    }

    public LobbyAdmission ReconnectMatch(MatchPresenceRequest request)
    {
        RequireRequest(request);
        return JoinLobby((request.LobbyCode, request.SessionToken), request.MatchId);
    }

    private LobbyAdmission JoinLobby((string LobbyCode, string SessionToken) access, Guid? matchId,
        [CallerMemberName] string operation = "")
    {
        SessionIdentity identity = ResolveIdentity(access.SessionToken, operation);
        string normalizedCode = RequireLobbyCode(access.LobbyCode);
        LobbyMember player = CreateMember(identity);
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

            EnsureReconnectMatchesLobby(lobby, player.PlayerId, matchId);

            if (lobby.Status != LobbyStatus.Waiting && !lobby.Contains(player.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
            }

            if (lobby.Contains(player.PlayerId))
            {
                LobbyConnection connection = new(player, GetCurrentCallback(), WatchCurrentSession());
                if (lobby.Status == LobbyStatus.InGame)
                {
                    if (!_gameOperations.TryReconnect(lobby, player.PlayerId))
                    {
                        if (matchId.HasValue)
                        {
                            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
                        }

                        throw CreateFault(ServiceErrorCodes.LobbyAlreadyStarted, ServiceMessageKeys.LobbyAlreadyStarted);
                    }
                }
                lobby.CancelInvitation(player.PlayerId);
                lobby.AddOrReplace(connection);
                response = CreateJoinResponse(lobby, player.PlayerId);
            }
            else
            {
                EnsureNewMemberCanJoin(lobby, player.PlayerId);
                response = CreateJoinResponse(lobby, player.PlayerId);
                callbacksToNotify = lobby.GetCallbacksExcept(player.PlayerId);
                LobbyConnection connection = new(player, GetCurrentCallback(), WatchCurrentSession());
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

    private static void EnsureReconnectMatchesLobby(LobbyState lobby, Guid playerId, Guid? matchId)
    {
        if (matchId.HasValue && (lobby.Status != LobbyStatus.InGame
            || lobby.MatchId != matchId.Value || !lobby.Contains(playerId)))
        {
            throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
        }
    }

    private void EnsureNewMemberCanJoin(LobbyState lobby, Guid playerId)
    {
        if (lobby.IsFull)
        {
            throw CreateFault(ServiceErrorCodes.LobbyFull, ServiceMessageKeys.LobbyFull);
        }

        if (!lobby.IsPublic && !lobby.IsInvited(playerId))
        {
            throw CreateFault(ServiceErrorCodes.LobbyInviteNotAllowed, ServiceMessageKeys.LobbyInviteNotAllowed);
        }

        if (_lobbies.Values.Any(otherLobby => otherLobby.Contains(playerId)))
        {
            throw CreateFault(ServiceErrorCodes.PlayerAlreadyInLobby, ServiceMessageKeys.PlayerAlreadyInLobby);
        }
    }

    // LeaveLobby is for the waiting room. Explicit match departure uses LeaveMatch.
    public bool LeaveLobby(string lobbyCode, string sessionToken)
    {
        SessionIdentity identity = ResolveIdentity(sessionToken);
        Guid playerId = identity.PlayerId;
        string normalizedCode = RequireLobbyCode(lobbyCode);
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
        RequireRequest(request);
        SessionIdentity identity = ResolveIdentity(request.SessionToken);
        string lobbyCode = RequireLobbyCode(request.LobbyCode);
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

    public MatchSyncUpdate GetMatchUpdate(MatchSyncRequest request)
    {
        RequireRequest(request);
        SessionIdentity identity = ResolveIdentity(request.SessionToken);
        MatchSyncState state;
        lock (_lobbyLock)
        {
            LobbyState lobby = RequireLobbyMember(request.LobbyCode, identity.PlayerId);
            EnsureReconnectMatchesLobby(lobby, identity.PlayerId, request.MatchId);
            MatchBoardSnapshot board = _gameOperations.GetBoard(lobby)
                ?? throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            MatchPrivateHand? hand = _gameOperations.GetPrivateHand(lobby.MatchId, identity.PlayerId);
            if (hand is null && !board.ExpelledPlayerIds.Contains(identity.PlayerId))
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            }

            state = new MatchSyncState(board, hand);
        }

        return MatchSynchronization.Compare(state, identity.PlayerId, request.Known);
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
        RequireRequest(request);
        MatchPlacement move = new(request.CardNumber, request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken, nameof(PlaceMatchCard)),
            (move.PilePosition, move.CardNumber), ApplyPlacement);

        MatchBoardActionKind? ApplyPlacement(Guid matchId, Guid playerId)
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
        RequireRequest(request);
        MatchDismantle move = new(request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken, nameof(DismantleMatchTower)),
            (move.PilePosition, 0), ApplyDismantle);

        MatchBoardActionKind? ApplyDismantle(Guid matchId, Guid playerId)
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
        RequireRequest(request);
        int[] modules = request.CardNumbers?.ToArray() ?? Array.Empty<int>();
        MatchMasterPlan move = new(modules, request.PilePosition, request.ExpectedRevision);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken, nameof(UseMatchMasterPlan)),
            (move.PilePosition, modules.LastOrDefault()), ApplyMasterPlan);

        MatchBoardActionKind? ApplyMasterPlan(Guid matchId, Guid playerId)
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
        RequireRequest(request);
        Guid matchToVote = request.MatchId;
        MatchExpulsionVote vote = new(
            request.TargetPlayerId, request.ExpectedVotingRevision, request.Decision, request.BallotId);

        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken, nameof(VoteToExpelMatchPlayer)), (0, 0), ApplyVote);

        MatchBoardActionKind? ApplyVote(Guid matchId, Guid voterId)
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

    public MatchBoardSnapshot ConfirmMatchPresence(MatchPresenceRequest request)
    {
        RequireRequest(request);
        return ApplyMatchBoardAction((request.LobbyCode, request.SessionToken, nameof(ConfirmMatchPresence)), (0, 0), Confirm);

        MatchBoardActionKind? Confirm(Guid matchId, Guid playerId)
        {
            if (matchId != request.MatchId)
            {
                throw CreateFault(ServiceErrorCodes.LobbyNotFound, ServiceMessageKeys.LobbyNotFound);
            }

            return _gameOperations.TryConfirmPresence(matchId, playerId)
                ? MatchBoardActionKind.PresenceConfirmed : null;
        }
    }

    private MatchBoardSnapshot ApplyMatchBoardAction(
        (string LobbyCode, string SessionToken, string Operation) access,
        (int PilePosition, int CardNumber) action,
        Func<Guid, Guid, MatchBoardActionKind?> apply)
    {
        SessionIdentity identity = ResolveIdentity(access.SessionToken, access.Operation);
        MatchBoardSnapshot board;
        MatchBoardActionKind? kind;
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

        if (kind.HasValue)
        {
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
                ActionKind = kind.Value,
                CardNumber = action.CardNumber,
                PilePosition = action.PilePosition
            };
            Notify(callbacks, CreateBoardNotification(changed));
        }

        return board;
    }

    public void ProcessExpiredMatchTurns()
    {
        List<(List<ILobbyCallback> Callbacks, MatchBoardChanged Notice)> notifications = new();
        lock (_lobbyLock)
        {
            _turnClockWriteFailures.RemoveWhere(matchId =>
                !_lobbies.Values.Any(lobby => lobby.MatchId == matchId));
            foreach (LobbyState lobby in _lobbies.Values)
            {
                if (lobby.Status == LobbyStatus.InGame
                    && TryExpireMatchTurn(lobby.MatchId) is Guid affectedPlayerId)
                {
                    MatchBoardSnapshot? board = _gameOperations.GetBoard(lobby);
                    MatchBoardMove? move = board?.RecentMoves.LastOrDefault();
                    if (board is not null && move is not null && move.PlayerId == affectedPlayerId)
                    {
                        MatchBoardChanged notice = new()
                        {
                            LobbyCode = lobby.LobbyCode,
                            MatchId = board.MatchId,
                            Revision = board.Revision,
                            PlayerUsername = move.PlayerUsername,
                            ActionKind = move.ActionKind
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

    private Guid? TryExpireMatchTurn(Guid matchId)
    {
        Guid? affectedPlayerId = null;
        try
        {
            affectedPlayerId = _gameOperations.TryExpireTurn(matchId);
            _turnClockWriteFailures.Remove(matchId);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            if (_turnClockWriteFailures.Add(matchId))
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                ILogger<LobbyService> logger = scope.ServiceProvider.GetRequiredService<ILogger<LobbyService>>();
                logger.LogError("Could not persist the turn expiration for match {MatchId}. The clock will retry.",
                    matchId);
            }
        }

        return affectedPlayerId;
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
        string normalizedCode = RequireLobbyCode(lobbyCode);
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

    public bool PublishChatMessage(LobbyChatMessage message)
    {
        LobbyState? lobby;
        lock (_lobbyLock)
        {
            _lobbies.TryGetValue(message.LobbyCode, out lobby);
        }

        bool accepted = false;
        if (lobby is not null)
        {
            // Keep callbacks in sequence for this lobby. The directory lock is used only to snapshot recipients.
            lock (lobby.ChatDispatchLock)
            {
                accepted = TryGetChatCallbacks(lobby, message, out List<ILobbyCallback> callbacks);
                if (accepted)
                {
                    message.Sequence = lobby.NextChatSequence();
                    Notify(callbacks, CreateChatNotification(message));
                }
            }
        }

        return accepted;
    }

    private bool TryGetChatCallbacks(LobbyState lobby, LobbyChatMessage message, out List<ILobbyCallback> callbacks)
    {
        callbacks = new();
        bool accepted = false;
        lock (_lobbyLock)
        {
            if (_lobbies.TryGetValue(message.LobbyCode, out LobbyState? current) && ReferenceEquals(current, lobby)
                && lobby.Contains(message.SenderId))
            {
                accepted = message.MatchId.HasValue
                    ? TryGetMatchChatCallbacks(lobby, message, out callbacks)
                    : lobby.Status == LobbyStatus.Waiting;
                if (accepted && !message.MatchId.HasValue)
                {
                    callbacks = lobby.GetCallbacksExcept(Guid.Empty);
                }
            }
        }

        return accepted;
    }

    private bool TryGetMatchChatCallbacks(LobbyState lobby, LobbyChatMessage message,
        out List<ILobbyCallback> callbacks)
    {
        callbacks = new();
        bool canSend = false;
        if (lobby.Status == LobbyStatus.InGame && lobby.MatchId == message.MatchId
            && !lobby.IsDisconnected(message.SenderId))
        {
            MatchBoardSnapshot? board = _gameOperations.GetBoard(lobby);
            if (board is { EndReason: MatchEndReason.None, TurnDeadlineUtc: not null }
                && board.Players.Exists(player => player.PlayerId == message.SenderId))
            {
                canSend = true;
                HashSet<Guid> activePlayers = board.Players.Select(player => player.PlayerId).ToHashSet();
                callbacks = lobby.GetConnectedCallbacks(activePlayers.Contains);
            }
        }

        return canSend;
    }

    private static void RequireRequest([NotNull] object? request)
    {
        if (request is null)
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }
    }

    private static string RequireLobbyCode(string? lobbyCode)
    {
        if (string.IsNullOrWhiteSpace(lobbyCode))
        {
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return lobbyCode.Trim();
    }

    private string? ReadCreationToken(string? bodyToken)
    {
        (string? token, string? rejection) = LobbySessionHeader.Read(
            OperationContext.Current?.IncomingMessageHeaders, bodyToken);
        if (rejection is not null)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            ILogger<LobbyService> logger = scope.ServiceProvider.GetRequiredService<ILogger<LobbyService>>();
            Dictionary<string, object> properties = new() { ["EventCategory"] = "Security" };
            using IDisposable? logScope = logger.BeginScope(properties);
            logger.LogWarning(InvalidSessionHeaderEvent, "Operation {Operation} rejected credentials. Reason: {Reason}.",
                nameof(CreateLobby), rejection);
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        return token;
    }

    private SessionIdentity ResolveIdentity(string? sessionToken, [CallerMemberName] string operation = "")
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        ISessionIdentityResolver resolver = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        return resolver.Require(sessionToken, operation);
    }

    private static LobbyMember CreateMember(SessionIdentity identity)
    {
        return new LobbyMember
        {
            PlayerId = identity.PlayerId,
            Username = identity.Username,
            AvatarId = identity.AvatarId,
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
            throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
        }

        lock (_lobbyLock)
        {
            if (_watchedSessions.Add(sessionId))
            {
                channel.Closed += OnChannelClosed;
                channel.Faulted += OnChannelClosed;
            }

            if (channel.State is CommunicationState.Closing or CommunicationState.Closed or CommunicationState.Faulted)
            {
                _watchedSessions.Remove(sessionId);
                channel.Closed -= OnChannelClosed;
                channel.Faulted -= OnChannelClosed;
                throw CreateFault(ServiceErrorCodes.InvalidData, ServiceMessageKeys.InvalidData);
            }
        }

        return sessionId;
    }

    private void OnChannelClosed(object? sender, EventArgs eventArgs)
    {
        if (sender is not IContextChannel channel)
        {
            return;
        }

        channel.Closed -= OnChannelClosed;
        channel.Faulted -= OnChannelClosed;
        string sessionId = channel.SessionId ?? string.Empty;
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
            catch (CommunicationException ex)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + ex.GetType().Name);
            }
            catch (TimeoutException ex)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + ex.GetType().Name);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Trace.TraceWarning("Lobby callback failed: " + ex.GetType().Name);
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
