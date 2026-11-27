using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Resources;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Dependencies;

namespace Vertigo.Client.UI.ViewModels;

public sealed partial class MatchViewModel : ViewModelBase, INavigationAware
{
    private readonly ITextLocalizer _text;
    private readonly ILobbyClient? _lobbies;
    private readonly INavigator? _navigation;
    private readonly PlayerSession? _session;
    private readonly ISessionGuard? _guard;
    private readonly IClientErrorText? _errors;
    private static readonly TimeSpan CountdownInterval = TimeSpan.FromMilliseconds(200);
    private readonly MatchBoardRetry _boardRetry;
    private readonly MatchBoardRevision _boardRevision = new();
    private readonly MatchTurnClock _turnClock;
    private readonly MatchTurnClock _presenceClock;
    private int? _presenceSeconds;
    private bool _wasExpelledForInactivity;
    private bool _needsReconnect;
    private bool _matchUnavailable;
    private int _connectionLossCount;
    private readonly MatchMasterPlanDraft _masterPlanDraft = new();
    private readonly MatchExpulsionVoting _voting = new();
    private DateTimeOffset? _turnDeadlineUtc;
    private int _masterPlanCount;
    private int _generation;
    private bool _isLoadingBoard;
    private bool _isExitConfirmationOpen;
    private bool _isLeavingMatch;
    private string _exitErrorMessage = string.Empty;
    private string _roundLabel = "—";
    private string? _turnPlayerName;
    private DispatcherTimer? _countdownTimer;
    private int? _turnSeconds;
    private bool _isMyTurn;
    private bool _hasCurrentHand;
    private bool _hasLegalPlacement;
    private bool _isSubmittingTurn;
    private string _errorMessage = string.Empty;
    private MatchCardRow? _selectedCard;
    private MatchPileRow? _selectedPile;
    private MatchPlayerRow? _selectedPlayer;
    private long _currentBoardRevision;
    private (SessionSnapshot Session, MatchSyncState State)? _syncBaseline;
    private bool _pendingBoardRefresh;
    private long _notifiedRevision;
    private IReadOnlyList<MatchBoardMove> _recentMoves = Array.Empty<MatchBoardMove>();
    private Guid _matchId;
    private string _lobbyCode = string.Empty;

    public MatchViewModel(ITextLocalizer text) : this(text, null, TimeProvider.System)
    {
    }

    internal MatchViewModel(ITextLocalizer text, PlayerSession? session, TimeProvider timeProvider)
    {
        _text = text;
        _boardRetry = new MatchBoardRetry(timeProvider);
        Results = new MatchResultsViewModel(text, timeProvider);
        ToggleRoundSummaryCommand = new RelayCommand(Results.ToggleRoundSummary);
        SendChatCommand = new AsyncRelayCommand(SendChatAsync, CanSendChat);
        _session = session;
        _turnClock = new MatchTurnClock(timeProvider);
        _presenceClock = new MatchTurnClock(timeProvider);
        ReconnectCommand = new AsyncRelayCommand(ReconnectAsync, () => NeedsReconnect && !IsLoadingBoard
            && !_isSubmittingTurn && !_isLeavingMatch);
        ConfirmPresenceCommand = new AsyncRelayCommand(ConfirmPresenceAsync, CanConfirmPresence);
        ReturnToMenuCommand = new RelayCommand(ReturnToMenu);
        RefreshBoardCommand = new AsyncRelayCommand(RefreshBoardAsync);
        PlaceCardCommand = new AsyncRelayCommand(PlaceCardAsync, CanPlaceSelectedCard);
        DismantleTowerCommand = new AsyncRelayCommand(DismantleTowerAsync, CanDismantleSelectedTower);
        AddMasterPlanModuleCommand = new RelayCommand(AddMasterPlanModule, CanAddMasterPlanModule);
        RemoveMasterPlanModuleCommand = new RelayCommand(RemoveMasterPlanModule, CanRemoveMasterPlanModule);
        PlayMasterPlanCommand = new AsyncRelayCommand(PlayMasterPlanAsync, CanPlayMasterPlan);
        ShowExpulsionPromptsCommand = new RelayCommand(ShowHiddenExpulsionPrompts);
        RequestExitCommand = new RelayCommand(RequestExit);
        CancelExitCommand = new RelayCommand(CancelExit, () => !_isLeavingMatch);
        ConfirmExitCommand = new AsyncRelayCommand(ConfirmExitAsync, CanConfirmExit);
    }

    public MatchViewModel(MatchBoardContext context, IChatClient chatClient)
        : this(context.Text, context.Session, TimeProvider.System)
    {
        _lobbies = context.Lobbies;
        _chatClient = chatClient;
        _guard = context.Guard;
        _errors = context.Errors;
        _navigation = context.Ui.Navigation;
    }

    public IRelayCommand ReturnToMenuCommand { get; }
    public IAsyncRelayCommand ReconnectCommand { get; }
    public IAsyncRelayCommand ConfirmPresenceCommand { get; }
    public IAsyncRelayCommand RefreshBoardCommand { get; }
    public IRelayCommand ToggleRoundSummaryCommand { get; }
    public IAsyncRelayCommand PlaceCardCommand { get; }
    public IAsyncRelayCommand DismantleTowerCommand { get; }
    public IRelayCommand AddMasterPlanModuleCommand { get; }
    public IRelayCommand RemoveMasterPlanModuleCommand { get; }
    public IAsyncRelayCommand PlayMasterPlanCommand { get; }
    public MatchPileRow? SelectedPile
    {
        get => _selectedPile;
        set
        {
            if (SetProperty(ref _selectedPile, value))
            {
                ClearMasterPlanDraft();
                RefreshTurnCommands();
            }
        }
    }

    public MatchPlayerRow? SelectedPlayer
    {
        get => _selectedPlayer;
        set => SetProperty(ref _selectedPlayer, value);
    }

    public bool IsLoadingBoard
    {
        get => _isLoadingBoard;
        private set
        {
            if (SetProperty(ref _isLoadingBoard, value))
            {
                RefreshTurnCommands();
            }
        }
    }

    public ObservableCollection<MatchMoveRow> RecentMoves { get; } = new();

    public MatchResultsViewModel Results { get; }
    public IReadOnlyList<MatchRoundSummary> RoundSummaries => Results.RoundSummaries;

    public Guid MatchId => _matchId;
    public string LobbyCode => _lobbyCode;

    public bool Activate(Guid matchId, string lobbyCode)
    {
        bool activated = matchId != Guid.Empty
            && !string.IsNullOrWhiteSpace(lobbyCode)
            && (_matchId != matchId
                || !string.Equals(_lobbyCode, lobbyCode, StringComparison.OrdinalIgnoreCase));
        if (activated)
        {
            Reset();
            _matchId = matchId;
            _lobbyCode = lobbyCode;
            OnPropertyChanged(nameof(MatchId));
            OnPropertyChanged(nameof(LobbyCode));
        }

        return activated;
    }

    public void Reset()
    {
        _generation++;
        _boardRevision.Reset();
        _matchId = Guid.Empty;
        _lobbyCode = string.Empty;
        SelectedPlayer = null;
        Players.Clear();
        _voting.ApplyBoard(null, null);
        _hiddenBallots.Clear();
        RefreshExpulsionPrompts();
        Piles.Clear();
        Hand.Clear();
        ChatMessages.Clear();
        DraftMessage = string.Empty;
        ChatErrorMessage = string.Empty;
        SelectedCard = null;
        SelectedPile = null;
        _currentBoardRevision = 0;
        _syncBaseline = null;
        _pendingBoardRefresh = false;
        _boardRetry.Reset();
        _notifiedRevision = 0;
        _recentMoves = Array.Empty<MatchBoardMove>();
        RecentMoves.Clear();
        Results.Reset();
        RefreshExitLabels();
        OnPropertyChanged(nameof(RoundSummaries));
        RoundLabel = "—";
        _turnPlayerName = null;
        _isMyTurn = false;
        _hasCurrentHand = false;
        _hasLegalPlacement = false;
        _isSubmittingTurn = false;
        _turnDeadlineUtc = null;
        SetMasterPlanCount(0);
        ClearMasterPlanDraft();
        _turnClock.SetDeadline(null, null);
        _presenceClock.SetDeadline(null, null);
        _wasExpelledForInactivity = false;
        SetMatchUnavailable(false);
        SetNeedsReconnect(false);
        RefreshTurnCountdown();
        OnPropertyChanged(nameof(CurrentTurnLabel));
        RefreshTurnCommands();
        ErrorMessage = string.Empty;
        SetLeavingMatch(false);
        IsExitConfirmationOpen = false;
        ExitErrorMessage = string.Empty;
        IsLoadingBoard = false;
        OnPropertyChanged(nameof(MatchId));
        OnPropertyChanged(nameof(LobbyCode));
    }

    public void OnNavigatedTo()
    {
        if (_lobbies is not null)
        {
            _lobbies.MatchBoardUpdated -= OnMatchBoardUpdated;
            _lobbies.MatchBoardUpdated += OnMatchBoardUpdated;
            _lobbies.ConnectionLost -= OnConnectionLost;
            _lobbies.ConnectionLost += OnConnectionLost;
            _lobbies.MessageReceived -= OnMatchChatReceived;
            _lobbies.MessageReceived += OnMatchChatReceived;
        }
        StartCountdownTimer();
        RefreshLocalization();
        _ = RefreshBoardAsync();
    }

    public void OnNavigatedFrom()
    {
        if (_lobbies is not null)
        {
            _lobbies.MatchBoardUpdated -= OnMatchBoardUpdated;
            _lobbies.ConnectionLost -= OnConnectionLost;
            _lobbies.MessageReceived -= OnMatchChatReceived;
        }
        StopCountdownTimer();
        ClearMasterPlanDraft();
        _generation++;
        _pendingBoardRefresh = false;
        _boardRetry.Reset();
        _hasCurrentHand = false;
        _voting.ApplyPrivateHand(null);
        _isSubmittingTurn = false;
        IsLoadingBoard = false;
        SetLeavingMatch(false);
        RefreshTurnCommands();
        IsExitConfirmationOpen = false;
        ExitErrorMessage = string.Empty;
    }

    private void OnConnectionLost(object? sender, EventArgs args)
    {
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (_generation == generation && _matchId != Guid.Empty)
            {
                SetNeedsReconnect(true);
            }
        });
    }

    private void OnMatchBoardUpdated(object? sender, MatchBoardChanged change)
    {
        // The duplex callback must return immediately: never call NetTCP from this thread.
        int generation = _generation;
        SessionSnapshot? session = _session?.GetActiveSession();
        Dispatcher.UIThread.Post(() =>
        {
            if (session is not null && IsCurrent(session, change.MatchId, generation)
                && MatchBoardNotificationRules.ShouldRefresh(
                    change,
                    new MatchBoardIdentity(_lobbyCode, _matchId),
                    Math.Max(_currentBoardRevision, _notifiedRevision)))
            {
                _notifiedRevision = Math.Max(_notifiedRevision, change.Revision);
                if (IsLoadingBoard)
                {
                    _pendingBoardRefresh = true;
                }
                else if (!_boardRetry.IsPending)
                {
                    _ = RefreshBoardAsync();
                }
            }
        });
    }

    public Task RefreshBoardAsync()
    {
        return NeedsReconnect || MatchUnavailable ? Task.CompletedTask : RequestBoardAsync(reconnect: false);
    }

    private Task ReconnectAsync()
    {
        return RequestBoardAsync(reconnect: true);
    }

    private async Task RequestBoardAsync(bool reconnect)
    {
        SessionSnapshot? original = _session?.GetActiveSession();
        Guid matchId = _matchId;
        string lobbyCode = _lobbyCode;
        int generation = _generation;
        if (!IsLoadingBoard && matchId != Guid.Empty && _guard is not null && _lobbies is not null
            && _guard.HasActiveSession() && original is not null)
        {
            BoardRequestIdentity request = new(original, matchId, generation);
            await LoadBoardAsync(_lobbies, (lobbyCode, reconnect), request);
        }
    }

    private async Task LoadBoardAsync(ILobbyClient lobbies, (string LobbyCode, bool Reconnect) options,
        BoardRequestIdentity request)
    {
        _boardRetry.BeginAttempt();
        _hasCurrentHand = false;
        IsLoadingBoard = true;
        int lossCount = _connectionLossCount;
        MatchSyncState? baseline = _syncBaseline is { } cached && ReferenceEquals(cached.Session, request.Session)
            ? cached.State : null;
        try
        {
            var state = await Task.Run(() => ReadBoard(lobbies, (options.LobbyCode, options.Reconnect, baseline), request));
            await Dispatcher.UIThread.InvokeAsync(() =>
                ApplyLoadedBoard(state, request, (options.Reconnect, lossCount)));
        }
        catch (FaultException<ServiceFault> exception) when (options.Reconnect
            && exception.Detail.ErrorCode == ServiceErrorCodes.LobbyNotFound)
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (IsCurrent(request.Session, request.MatchId, request.Generation))
                {
                    SetMatchUnavailable(true);
                    SetNeedsReconnect(false);
                    ErrorMessage = string.Empty;
                }
            });
        }
        catch (Exception exception)
        {
            await ReportBoardFailureAsync(exception, request, isBoardRead: !options.Reconnect);
        }
        finally
        {
            await Dispatcher.UIThread.InvokeAsync(() => CompleteBoardRefresh(request));
        }
    }

    private void ApplyLoadedBoard(
        (MatchSyncState Snapshot, long ReceivedAt, long? SupersededRevision) state,
        BoardRequestIdentity request,
        (bool Requested, int LossCount) reconnect)
    {
        if (IsCurrent(request.Session, request.MatchId, request.Generation))
        {
            MatchBoardSnapshot board = state.Snapshot.Board;
            long? supersededRevision = reconnect.Requested && reconnect.LossCount == _connectionLossCount
                ? state.SupersededRevision : null;
            ApplySynchronizedBoard(state.Snapshot, state.ReceivedAt, supersededRevision);
            if (board.Revision == _currentBoardRevision)
            {
                _boardRetry.Reset();
                _syncBaseline = (request.Session, state.Snapshot);
            }

            if (reconnect.Requested && reconnect.LossCount == _connectionLossCount
                && board.Revision == _currentBoardRevision
                && (_hasCurrentHand || board.ExpelledPlayerIds.Contains(request.Session.PlayerId)))
            {
                SetNeedsReconnect(false);
            }
            else if (!reconnect.Requested && NeedsReconnect)
            {
                _hasCurrentHand = false;
            }
        }
    }

    internal void ApplySynchronizedBoard(MatchSyncState state, long receivedAt, long? supersededRevision)
    {
        if (state.Board.MatchId == _matchId && supersededRevision == _currentBoardRevision
            && state.Board.Revision < _currentBoardRevision)
        {
            _boardRevision.Reset();
            _currentBoardRevision = 0;
            _notifiedRevision = state.Board.Revision;
        }

        ApplyBoardAndHand(state.Board, state.Hand, receivedAt);
    }

    private (MatchSyncState Snapshot, long ReceivedAt, long? SupersededRevision) ReadBoard(
        ILobbyClient lobbies, (string LobbyCode, bool Reconnect, MatchSyncState? Baseline) options,
        BoardRequestIdentity request)
    {
        if (options.Reconnect)
        {
            lobbies.ReconnectMatch(CreatePresenceRequest(options.LobbyCode, request.MatchId, request.Session.Token));
        }

        MatchSyncRequest sync = new()
        {
            LobbyCode = options.LobbyCode,
            MatchId = request.MatchId,
            SessionToken = request.Session.Token,
            Known = options.Baseline is null ? null : MatchSynchronization.Describe(options.Baseline)
        };
        MatchSyncUpdate update = lobbies.GetMatchUpdate(sync);
        long sampleTimestamp = _turnClock.GetTimestamp();
        if (update.MatchId != request.MatchId)
        {
            throw new InvalidDataException("The match update belongs to a different match.");
        }

        MatchSyncState state = MatchSynchronization.Apply(update, options.Baseline, request.Session.PlayerId);
        long? supersededRevision = update.BaseRevision is null && sync.Known?.Revision > update.Revision
            ? sync.Known.Revision : null;
        return (state, sampleTimestamp, supersededRevision);
    }

    internal void ApplyBoardAndHand(MatchBoardSnapshot board, MatchPrivateHand? hand, long? receivedAt = null)
    {
        SessionSnapshot? session = _session?.GetActiveSession();
        if (session is not null && _matchId != Guid.Empty && board.MatchId == _matchId
            && board.Revision >= _currentBoardRevision)
        {
            bool expelled = board.ExpelledPlayerIds.Contains(session.PlayerId);
            bool validHand = IsValidHand(hand, session.PlayerId, board.Revision);
            ApplyBoardAndUpdateClock(board, receivedAt);

            ReplacePrivateHand(!expelled && validHand ? hand : null);
            ErrorMessage = expelled || validHand ? string.Empty : _text.Get("Error.Unknown");
        }
    }

    private bool IsValidHand(MatchPrivateHand? hand, Guid playerId, long boardRevision)
    {
        return hand is not null && hand.MatchId == _matchId && hand.PlayerId == playerId
            && (hand.Revision is null || hand.Revision == boardRevision);
    }

    private void ApplyBoardAndUpdateClock(MatchBoardSnapshot board, long? receivedAt)
    {
        if (!ApplyBoard(board, receivedAt) && board.ServerTimeUtc.HasValue)
        {
            SetBoardClocks(board, _session?.GetActiveSession()?.PlayerId, receivedAt);
            RefreshTurnCountdown();
        }
    }

    private void CompleteBoardRefresh(BoardRequestIdentity request)
    {
        if (request.Generation == _generation)
        {
            IsLoadingBoard = false;
            bool needsRefresh = _pendingBoardRefresh && (_notifiedRevision > _currentBoardRevision || !_hasCurrentHand);
            _pendingBoardRefresh = false;
            if (needsRefresh && !_boardRetry.IsPending && IsCurrent(request.Session, request.MatchId, request.Generation))
            {
                _ = RefreshBoardAsync();
            }
        }
    }

    private async Task ReportBoardFailureAsync(Exception exception, BoardRequestIdentity request, bool isBoardRead)
    {
        Debug.WriteLine($"Match operation failed: {exception.GetType().Name} (0x{exception.HResult:X8}).");
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (IsCurrent(request.Session, request.MatchId, request.Generation))
            {
                bool awaitingNotifiedRevision = _notifiedRevision > _currentBoardRevision;
                if (!NeedsReconnect && IsRetryableBoardFailure(exception, isBoardRead, awaitingNotifiedRevision))
                {
                    _boardRetry.Schedule();
                }
                else if (exception is TimeoutException or (CommunicationException and not FaultException))
                {
                    SetNeedsReconnect(true);
                }
                else if (isBoardRead)
                {
                    _boardRetry.Reset();
                }

                ErrorMessage = _boardRetry.IsPending ? _text.Get("Match.Connection.SyncRetry")
                    : _errors?.FromException(exception, "Error.Unknown") ?? _text.Get("Error.Unknown");
                if (_errors?.IsUnauthorized(exception) == true)
                {
                    _guard?.Invalidate(request.Session);
                }
            }
        });
    }

    internal static bool IsRetryableBoardFailure(Exception exception, bool isBoardRead, bool awaitingNotifiedRevision)
    {
        return isBoardRead && (exception is FaultException<ServiceFault>
            { Detail.ErrorCode: ServiceErrorCodes.Unexpected }
            || (exception is TimeoutException && awaitingNotifiedRevision));
    }

    public bool ApplyBoard(MatchBoardSnapshot board)
    {
        return ApplyBoard(board, null);
    }

    private bool ApplyBoard(MatchBoardSnapshot board, long? receivedAt)
    {
        bool accepted = _boardRevision.TryAdvance(board, _matchId);
        if (accepted)
        {
            _hasCurrentHand = false;
            Guid? localPlayerId = _session?.GetActiveSession()?.PlayerId;
            _voting.ApplyBoard(board, localPlayerId);
            RefreshPlayers();
            RefreshExpulsionPrompts();

            _currentBoardRevision = board.Revision;
            UpdatePiles(board.Piles);

            int previousRoundCount = RoundSummaries.Count;
            Results.ApplyBoard(board, localPlayerId);
            RefreshExitLabels();
            if (previousRoundCount != RoundSummaries.Count)
            {
                OnPropertyChanged(nameof(RoundSummaries));
            }

            _recentMoves = board.RecentMoves;
            RefreshMoveHistory();
            RoundLabel = board.RoundNumber?.ToString() ?? "—";
            MatchBoardPlayer? current = board.Players.Find(player =>
                player.PlayerId == board.CurrentTurnPlayerId);
            _turnPlayerName = current?.Username;
            _isMyTurn = current is not null && current.PlayerId == localPlayerId;
            if (!_isMyTurn || _turnDeadlineUtc != board.TurnDeadlineUtc)
            {
                SelectedPile = null;
                ClearMasterPlanDraft();
            }

            _turnDeadlineUtc = board.TurnDeadlineUtc;
            SetBoardClocks(board, localPlayerId, receivedAt);
            _wasExpelledForInactivity = localPlayerId is Guid playerId && board.InactivePlayerIds.Contains(playerId);
            RefreshTurnCountdown();
            RefreshPresenceNotice();
            OnPropertyChanged(nameof(CurrentTurnLabel));
            RefreshTurnCommands();
            ErrorMessage = string.Empty;
        }

        return accepted;
    }

    private void RefreshPlayers()
    {
        Guid? selectedPlayerId = SelectedPlayer?.PlayerId;
        Players.Clear();
        if (_voting.Board is MatchBoardSnapshot board)
        {
            Guid? localPlayerId = _session?.GetActiveSession()?.PlayerId;
            foreach (MatchBoardPlayer player in board.Players)
            {
                Players.Add(CreatePlayerRow(player, board.CurrentTurnPlayerId, localPlayerId));
            }
        }

        SelectedPlayer = Players.FirstOrDefault(player => player.PlayerId == selectedPlayerId);
    }

    private MatchPlayerRow CreatePlayerRow(MatchBoardPlayer player, Guid activePlayerId, Guid? localPlayerId)
    {
        Guid targetId = player.PlayerId;
        MatchExpulsionVoteSummary? ballot = _voting.FindBallot(targetId);
        long ballotId = ballot?.BallotId ?? 0;
        bool legacyBallot = ballot is { BallotId: 0, IsRejected: false };
        bool canShowVote = _voting.IsEligibleTarget(targetId) && (legacyBallot || CanRequestExpulsion(targetId, ballotId));
        MatchExpulsionDecision decision = legacyBallot ? MatchExpulsionDecision.Support : MatchExpulsionDecision.Request;
        return new MatchPlayerRow
        {
            PlayerId = targetId,
            Username = player.Username,
            Avatar = AvatarCatalog.Resolve(player.AvatarId),
            FloorsBuilt = player.FloorsBuilt,
            IsCurrentTurn = targetId == activePlayerId,
            IsMe = targetId == localPlayerId,
            VoteText = _text.Get("Match.Expulsion.Vote"),
            VoteCommand = canShowVote
                ? new AsyncRelayCommand(
                    () => SubmitExpulsionAsync(targetId, decision, ballotId),
                    () => CanSubmitExpulsionDecision(targetId, decision, ballotId))
                : null
        };
    }

    private static MatchPresenceRequest CreatePresenceRequest(string lobbyCode, Guid matchId, string token)
    {
        return new MatchPresenceRequest { LobbyCode = lobbyCode, MatchId = matchId, SessionToken = token };
    }

    public bool NeedsReconnect => _needsReconnect && !MatchUnavailable;
    public bool MatchUnavailable => _matchUnavailable;
    public string MatchUnavailableText => _text.Get("Match.Connection.Unavailable");
    public string ReturnToMenuText => _text.Get("Match.Exit.ReturnHome");

    private void SetMatchUnavailable(bool value)
    {
        if (SetProperty(ref _matchUnavailable, value, nameof(MatchUnavailable)))
        {
            OnPropertyChanged(nameof(NeedsReconnect));
            RefreshPresenceNotice();
            RefreshTurnCommands();
        }
    }
    public string ReconnectText => _text.Get("Match.Connection.Reconnect");
    public string ConnectionText => _text.Get("Match.Connection.Lost");
    public string ConfirmPresenceText => _text.Get("Match.Presence.Confirm");
    public bool ShowConfirmPresence => _presenceSeconds is > 0 && !_isMyTurn && !NeedsReconnect && !MatchUnavailable;
    public bool HasPresenceNotice => !MatchUnavailable && (_wasExpelledForInactivity || _presenceSeconds.HasValue);
    public string PresenceNotice => _wasExpelledForInactivity
        ? _text.Get("Match.Presence.Expelled")
        : _presenceSeconds == 0 ? _text.Get("Match.Presence.Pending")
        : string.Format(_text.Get(_isMyTurn ? "Match.Presence.Play" : "Match.Presence.Skipped"), _presenceSeconds);

    private void SetNeedsReconnect(bool value)
    {
        if (value)
        {
            if (_boardRetry.IsPending)
            {
                ErrorMessage = string.Empty;
            }

            _boardRetry.Reset();
            _connectionLossCount++;
            _hasCurrentHand = false;
        }

        if (SetProperty(ref _needsReconnect, value, nameof(NeedsReconnect)))
        {
            RefreshPresenceNotice();
            RefreshTurnCommands();
        }
    }

    private void RefreshPresenceNotice()
    {
        OnPropertyChanged(nameof(HasPresenceNotice));
        OnPropertyChanged(nameof(PresenceNotice));
        OnPropertyChanged(nameof(ShowConfirmPresence));
        ConfirmPresenceCommand.NotifyCanExecuteChanged();
    }

    private bool CanConfirmPresence()
    {
        return ShowConfirmPresence && _presenceClock.GetRemainingSeconds() is > 0
            && !IsLoadingBoard && !_isSubmittingTurn && !_isLeavingMatch;
    }

    private Task ConfirmPresenceAsync()
    {
        string lobbyCode = _lobbyCode;
        Guid matchId = _matchId;
        return CanConfirmPresence()
            ? SubmitTurnAsync((lobbies, token) =>
                lobbies.ConfirmMatchPresence(CreatePresenceRequest(lobbyCode, matchId, token)))
            : Task.CompletedTask;
    }

    private void UpdatePiles(IReadOnlyList<MatchBoardPile> piles)
    {
        bool unchanged = Piles.Select(pile => (pile.Position, pile.TopCardNumber, pile.Count))
            .SequenceEqual(piles.Select(pile => (pile.Position, pile.TopCardNumber, pile.CardCount)));
        if (!unchanged)
        {
            SelectedPile = null;
            Piles.Clear();
            foreach (MatchBoardPile pile in piles)
            {
                Piles.Add(new MatchPileRow
                {
                    Position = pile.Position,
                    TopCardNumber = pile.TopCardNumber,
                    Label = pile.Position.ToString(),
                    Count = pile.CardCount,
                    TopCardText = pile.TopCardNumber?.ToString() ?? "—"
                });
            }
        }
    }

    private bool CanSubmitTurn()
    {
        return _isMyTurn && _hasCurrentHand && !NeedsReconnect && !MatchUnavailable && !IsLoadingBoard
            && !_isSubmittingTurn && !_isLeavingMatch
            && _turnClock.GetRemainingSeconds() is > 0;
    }

    private bool CanPlaceSelectedCard()
    {
        return CanSubmitTurn() && _masterPlanDraft.Modules.Count == 0
            && SelectedPile is { } pile && Piles.Contains(pile)
            && SelectedCard is { } card && Hand.Contains(card)
            && MatchTurnRules.CanPlace(pile.TopCardNumber, card.CardNumber);
    }

    private bool CanDismantleSelectedTower()
    {
        return CanSubmitTurn() && !_hasLegalPlacement
            && SelectedPile is { TopCardNumber: not null } pile && Piles.Contains(pile);
    }

    private Task PlaceCardAsync()
    {
        MatchPileRow? pile = SelectedPile;
        int? card = SelectedCard?.CardNumber;
        string lobbyCode = _lobbyCode;
        long revision = _currentBoardRevision;
        return CanPlaceSelectedCard() && pile is not null && card is int module
            ? SubmitTurnAsync((lobbies, token) => lobbies.PlaceMatchCard(new MatchPlaceRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                CardNumber = module,
                PilePosition = pile.Position,
                ExpectedRevision = revision
            }))
            : Task.CompletedTask;
    }

    private Task DismantleTowerAsync()
    {
        MatchPileRow? pile = SelectedPile;
        string lobbyCode = _lobbyCode;
        long revision = _currentBoardRevision;
        return CanDismantleSelectedTower() && pile is not null
            ? SubmitTurnAsync((lobbies, token) => lobbies.DismantleMatchTower(new MatchDismantleRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                PilePosition = pile.Position,
                ExpectedRevision = revision
            }))
            : Task.CompletedTask;
    }

    private bool CanAddMasterPlanModule()
    {
        return CanSubmitTurn() && HasMasterPlans
            && SelectedPile is { } pile && Piles.Contains(pile)
            && SelectedCard is { } card && Hand.Contains(card)
            && _masterPlanDraft.CanAppend(pile.TopCardNumber, card.CardNumber);
    }

    private void AddMasterPlanModule()
    {
        if (CanAddMasterPlanModule() && SelectedCard?.CardNumber is int card
            && _masterPlanDraft.TryAppend(SelectedPile?.TopCardNumber, card))
        {
            OnMasterPlanDraftChanged();
        }
    }

    private bool CanRemoveMasterPlanModule()
    {
        return !_isSubmittingTurn && _masterPlanDraft.Modules.Count > 0;
    }

    private void RemoveMasterPlanModule()
    {
        if (CanRemoveMasterPlanModule() && _masterPlanDraft.RemoveLast())
        {
            OnMasterPlanDraftChanged();
        }
    }

    private bool CanPlayMasterPlan()
    {
        return CanSubmitTurn() && HasMasterPlans && _masterPlanDraft.CanPlay
            && SelectedPile is { TopCardNumber: not null } pile && Piles.Contains(pile);
    }

    private Task PlayMasterPlanAsync()
    {
        MatchPileRow? pile = SelectedPile;
        string lobbyCode = _lobbyCode;
        long revision = _currentBoardRevision;
        List<int> modules = new(_masterPlanDraft.Modules);

        return CanPlayMasterPlan() && pile is not null
            ? SubmitTurnAsync((lobbies, token) => lobbies.UseMatchMasterPlan(new MatchMasterPlanRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                CardNumbers = modules,
                PilePosition = pile.Position,
                ExpectedRevision = revision
            }))
            : Task.CompletedTask;
    }

    private async Task SubmitTurnAsync(Func<ILobbyClient, string, MatchBoardSnapshot> submit)
    {
        SessionSnapshot? session = _session?.GetActiveSession();
        if (session is not null && _lobbies is not null && _guard?.HasActiveSession() == true)
        {
            BoardRequestIdentity request = new(session, _matchId, _generation);
            _isSubmittingTurn = true;
            RefreshTurnCommands();
            ErrorMessage = string.Empty;
            try
            {
                MatchBoardSnapshot board = await Task.Run(() => submit(_lobbies, session.Token));
                await Dispatcher.UIThread.InvokeAsync(() => ApplyTurnResult(board, request));
            }
            catch (FaultException<ServiceFault> exception)
                when (exception.Detail.ErrorCode is "MATCH_STALE_REVISION" or "MATCH_TURN_EXPIRED"
                    or "MATCH_STALE_VOTE" or "MATCH_ALREADY_VOTED")
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (IsCurrent(request.Session, request.MatchId, request.Generation))
                    {
                        _hasCurrentHand = false;
                        if (IsLoadingBoard)
                        {
                            _pendingBoardRefresh = true;
                        }
                        else
                        {
                            _ = RefreshBoardAsync();
                        }
                    }
                });
                await ReportBoardFailureAsync(exception, request, isBoardRead: false);
            }
            catch (Exception exception)
            {
                await ReportBoardFailureAsync(exception, request, isBoardRead: false);
            }
            finally
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (request.Generation == _generation)
                    {
                        _isSubmittingTurn = false;
                        RefreshTurnCommands();
                    }
                });
            }
        }
    }

    private void ApplyTurnResult(MatchBoardSnapshot board, BoardRequestIdentity request)
    {
        if (IsCurrent(request.Session, request.MatchId, request.Generation))
        {
            ApplyBoard(board);
            if (IsLoadingBoard)
            {
                _pendingBoardRefresh = true;
            }
            else
            {
                _ = RefreshBoardAsync();
            }
        }
    }

    private void ReplacePrivateHand(MatchPrivateHand? hand)
    {
        Hand.Clear();
        SelectedCard = null;
        _hasCurrentHand = hand is not null;
        _voting.ApplyPrivateHand(hand);
        RefreshExpulsionPrompts();
        SetMasterPlanCount(hand?.MasterPlans ?? 0);
        if (hand is null || !HasMasterPlans || !_masterPlanDraft.IsStillInHand(hand.Cards))
        {
            ClearMasterPlanDraft();
        }

        _hasLegalPlacement = hand is not null
            && MatchTurnRules.HasLegalPlacement(Piles.Select(pile => pile.TopCardNumber), hand.Cards);
        if (hand is not null)
        {
            foreach (int card in hand.Cards)
            {
                Hand.Add(new MatchCardRow
                {
                    DisplayValue = card.ToString(),
                    CardNumber = card,
                    PlanOrder = _masterPlanDraft.GetOrder(card)
                });
            }
        }

        RefreshTurnCommands();
    }

    private void RefreshTurnCommands()
    {
        SendChatCommand.NotifyCanExecuteChanged();
        ReconnectCommand.NotifyCanExecuteChanged();
        ConfirmPresenceCommand.NotifyCanExecuteChanged();
        ConfirmExitCommand.NotifyCanExecuteChanged();
        PlaceCardCommand.NotifyCanExecuteChanged();
        DismantleTowerCommand.NotifyCanExecuteChanged();
        AddMasterPlanModuleCommand.NotifyCanExecuteChanged();
        RemoveMasterPlanModuleCommand.NotifyCanExecuteChanged();
        PlayMasterPlanCommand.NotifyCanExecuteChanged();
        foreach (MatchExpulsionPrompt prompt in ExpulsionPrompts)
        {
            prompt.AcceptCommand.NotifyCanExecuteChanged();
            prompt.RejectCommand.NotifyCanExecuteChanged();
        }

        foreach (MatchPlayerRow player in Players)
        {
            player.VoteCommand?.NotifyCanExecuteChanged();
        }
    }

    private void SetMasterPlanCount(int count)
    {
        if (SetProperty(ref _masterPlanCount, count, nameof(MasterPlanCountLabel)))
        {
            OnPropertyChanged(nameof(HasMasterPlans));
            OnPropertyChanged(nameof(MasterPlanCount));
        }
    }

    private void ClearMasterPlanDraft()
    {
        if (_masterPlanDraft.Modules.Count > 0)
        {
            _masterPlanDraft.Clear();
            OnMasterPlanDraftChanged();
        }
    }

    private void OnMasterPlanDraftChanged()
    {
        foreach (MatchCardRow card in Hand)
        {
            card.PlanOrder = _masterPlanDraft.GetOrder(card.CardNumber);
        }

        OnPropertyChanged(nameof(MasterPlanPreview));
        OnPropertyChanged(nameof(HasMasterPlanDraft));
        OnPropertyChanged(nameof(MasterPlanSteps));
        RefreshTurnCommands();
    }

    private void StartCountdownTimer()
    {
        if (_countdownTimer is null)
        {
            _countdownTimer = new DispatcherTimer { Interval = CountdownInterval };
            _countdownTimer.Tick += OnCountdownTick;
            _countdownTimer.Start();
        }

        RefreshTurnCountdown();
    }

    private void StopCountdownTimer()
    {
        if (_countdownTimer is not null)
        {
            _countdownTimer.Stop();
            _countdownTimer.Tick -= OnCountdownTick;
            _countdownTimer = null;
        }
    }

    private void OnCountdownTick(object? sender, EventArgs args)
    {
        RefreshTurnCountdown();
        Results.UpdateRoundPreview();
        if (!IsLoadingBoard && !NeedsReconnect && !MatchUnavailable && !_isLeavingMatch && !_isSubmittingTurn
            && _boardRetry.IsDue)
        {
            _ = RefreshBoardAsync();
        }
    }

    private void SetBoardClocks(MatchBoardSnapshot board, Guid? playerId, long? receivedAt)
    {
        _turnClock.SetDeadline(board.TurnDeadlineUtc, board.ServerTimeUtc, receivedAt);
        DateTimeOffset? presenceDeadline = playerId is Guid id
            && board.PresenceDeadlinesUtc.TryGetValue(id, out DateTimeOffset deadline) ? deadline : null;
        _presenceClock.SetDeadline(presenceDeadline, board.ServerTimeUtc, receivedAt);
    }

    private void RefreshTurnCountdown()
    {
        int? presenceSeconds = _presenceClock.GetRemainingSeconds();
        if (presenceSeconds != _presenceSeconds)
        {
            _presenceSeconds = presenceSeconds;
            RefreshPresenceNotice();
        }

        if (SetProperty(ref _turnSeconds, _turnClock.GetRemainingSeconds(), nameof(TurnSecondsLabel)))
        {
            OnPropertyChanged(nameof(HasTurnCountdown));
            if (_turnSeconds == 0)
            {
                ClearMasterPlanDraft();
            }

            RefreshTurnCommands();
        }
    }

    private sealed record BoardRequestIdentity(SessionSnapshot Session, Guid MatchId, int Generation);

    private bool IsCurrent(SessionSnapshot original, Guid matchId, int generation)
    {
        return generation == _generation && _matchId == matchId
            && _guard is not null && _guard.IsCurrent(original);
    }

    public ObservableCollection<MatchPlayerRow> Players { get; } = new();
    public ObservableCollection<MatchPileRow> Piles { get; } = new();
    public ObservableCollection<MatchCardRow> Hand { get; } = new();

    public string RoundLabel
    {
        get => _roundLabel;
        set => SetProperty(ref _roundLabel, value);
    }

    public string CurrentTurnLabel => _isMyTurn ? _text.Get("Match.Turn.YourTurn")
        : _turnPlayerName is null ? "—" : string.Format(_text.Get("Match.Turn.OtherPlayer"), _turnPlayerName);

    public string TurnSecondsLabel => _turnSeconds is int seconds
        ? string.Format(_text.Get("Match.Turn.Seconds"), seconds) : string.Empty;

    public bool HasTurnCountdown => _turnSeconds.HasValue;

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public MatchCardRow? SelectedCard
    {
        get => _selectedCard;
        set
        {
            if (SetProperty(ref _selectedCard, value))
            {
                RefreshTurnCommands();
            }
        }
    }

    public bool IsExitConfirmationOpen
    {
        get => _isExitConfirmationOpen;
        private set
        {
            if (SetProperty(ref _isExitConfirmationOpen, value))
            {
                RefreshTurnCommands();
            }
        }
    }

    public string ExitErrorMessage
    {
        get => _exitErrorMessage;
        private set
        {
            if (SetProperty(ref _exitErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasExitError));
            }
        }
    }

    public bool HasExitError => !string.IsNullOrEmpty(ExitErrorMessage);

    public string TitleText => _text.Get("Match.Title");
    public string RoundText => _text.Get("Match.Round");
    public string PlayersText => _text.Get("Match.Players");
    public string FloorsText => _text.Get("Match.Floors");
    public string BoardText => _text.Get("Match.Board");
    public string BoardHintText => _text.Get("Match.BoardHint");
    public string HandText => _text.Get("Match.Hand");
    public int MasterPlanCount => _masterPlanCount;
    public string MasterPlanTitle => _text.Get("Match.MasterPlan.Title");
    public bool HasMasterPlans => MasterPlanCount > 0;
    public string MasterPlanCountLabel => string.Format(_text.Get("Match.MasterPlan.Count"), _masterPlanCount);
    public bool HasMasterPlanDraft => _masterPlanDraft.Modules.Count > 0;
    public string MasterPlanPreview => SelectedPile?.TopCardNumber is int top && _masterPlanDraft.Modules.Count > 0
        ? string.Join(" → ", _masterPlanDraft.Modules.Prepend(top)) : _text.Get("Match.MasterPlan.Hint");
    public IReadOnlyList<MatchMasterPlanStep> MasterPlanSteps =>
        HasMasterPlanDraft && SelectedPile?.TopCardNumber is int top
            ? _masterPlanDraft.Modules.Select(number => new MatchMasterPlanStep(number, IsTowerTop: false))
                .Prepend(new MatchMasterPlanStep(top, IsTowerTop: true)).ToArray()
            : Array.Empty<MatchMasterPlanStep>();

    public string AddMasterPlanModuleText => _text.Get("Match.MasterPlan.AddModule");
    public string RemoveMasterPlanModuleText => _text.Get("Match.MasterPlan.RemoveModule");
    public string PlayMasterPlanText => _text.Get("Match.MasterPlan.Play");
    public string RecentMovesText => _text.Get("Match.RecentMoves");
    public string ChatText => _text.Get("Match.Chat");
    public string MessageHintText => _text.Get("Match.MessageHint");
    public string SendText => _text.Get("Match.Send");
    public string PlayCardText => _text.Get("Match.PlayCard");
    public string DismantleTowerText => _text.Get("Match.DismantleTower");
    private bool IsReturningToMenu => Results.HasFinalResult
        || (_session?.GetActiveSession()?.PlayerId is Guid playerId
            && _voting.Board?.ExpelledPlayerIds.Contains(playerId) == true);

    public string ExitText => _text.Get(IsReturningToMenu ? "Match.Exit.ReturnHome" : "Match.Exit");
    public string ExitConfirmTitleText => _text.Get(IsReturningToMenu
        ? "Match.Exit.ReturnHome" : "Match.ExitConfirmTitle");
    public string ExitConfirmBodyText => _text.Get(IsReturningToMenu
        ? "Match.Exit.FinalBody" : "Match.ExitConfirmBody");
    public string CancelText => _text.Get("Match.Cancel");
    public string ConfirmText => _text.Get("Match.Confirm");
    public string ErrorTitleText => _text.Get("Room.ErrorHeading");

    public IRelayCommand RequestExitCommand { get; }
    public IRelayCommand CancelExitCommand { get; }
    public IAsyncRelayCommand ConfirmExitCommand { get; }

    public void RefreshLocalization()
    {
        Results.RefreshLocalization();
        OnPropertyChanged(nameof(ReconnectText));
        OnPropertyChanged(nameof(ConnectionText));
        OnPropertyChanged(nameof(MatchUnavailableText));
        OnPropertyChanged(nameof(ReturnToMenuText));
        OnPropertyChanged(nameof(ConfirmPresenceText));
        RefreshPresenceNotice();
        RefreshPlayers();
        RefreshExpulsionPrompts();
        OnPropertyChanged(nameof(ShowExpulsionPromptsText));
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(RoundText));
        OnPropertyChanged(nameof(CurrentTurnLabel));
        OnPropertyChanged(nameof(TurnSecondsLabel));
        OnPropertyChanged(nameof(PlayersText));
        OnPropertyChanged(nameof(FloorsText));
        OnPropertyChanged(nameof(BoardText));
        OnPropertyChanged(nameof(BoardHintText));
        OnPropertyChanged(nameof(HandText));
        OnPropertyChanged(nameof(MasterPlanCountLabel));
        OnPropertyChanged(nameof(MasterPlanTitle));
        OnPropertyChanged(nameof(MasterPlanPreview));
        OnPropertyChanged(nameof(AddMasterPlanModuleText));
        OnPropertyChanged(nameof(RemoveMasterPlanModuleText));
        OnPropertyChanged(nameof(PlayMasterPlanText));
        OnPropertyChanged(nameof(ChatText));
        OnPropertyChanged(nameof(RecentMovesText));
        RefreshMoveHistory();
        OnPropertyChanged(nameof(MessageHintText));
        OnPropertyChanged(nameof(SendText));
        OnPropertyChanged(nameof(PlayCardText));
        OnPropertyChanged(nameof(DismantleTowerText));
        RefreshExitLabels();
        OnPropertyChanged(nameof(CancelText));
        OnPropertyChanged(nameof(ConfirmText));
        OnPropertyChanged(nameof(ErrorTitleText));
    }

    private void RefreshMoveHistory()
    {
        MatchMoveRow[] rows = _recentMoves.Select(move => new MatchMoveRow(FormatMove(move), move.Revision)).ToArray();
        if (!RecentMoves.SequenceEqual(rows))
        {
            RecentMoves.Clear();
            foreach (MatchMoveRow row in rows)
            {
                RecentMoves.Add(row);
            }
        }
    }

    private string FormatMove(MatchBoardMove move)
    {
        string label;
        if (move.ActionKind == MatchBoardActionKind.Dismantle)
        {
            label = string.Format(_text.Get("Match.Move.Dismantle"), move.PlayerUsername, move.PilePosition);
        }
        else if (move.ActionKind == MatchBoardActionKind.TurnExpired)
        {
            label = string.Format(_text.Get("Match.Move.TurnExpired"), move.PlayerUsername);
        }
        else if (move.ActionKind == MatchBoardActionKind.TurnSkipped)
        {
            label = string.Format(_text.Get("Match.Move.TurnSkipped"), move.PlayerUsername);
        }
        else if (move.ActionKind == MatchBoardActionKind.PresenceConfirmed)
        {
            label = string.Format(_text.Get("Match.Move.PresenceConfirmed"), move.PlayerUsername);
        }
        else if (move.ActionKind is MatchBoardActionKind.LeftMatch or MatchBoardActionKind.ExpelledByVote)
        {
            label = string.Format(_text.Get("Match.Move.LeftMatch"), move.PlayerUsername);
        }
        else
        {
            string key = move.ActionKind == MatchBoardActionKind.MasterPlan
                ? "Match.Move.MasterPlan" : "Match.Move.Placement";
            IEnumerable<int> chain = move.PreviousTop is int top
                ? move.CardNumbers.Prepend(top) : move.CardNumbers;
            label = string.Format(_text.Get(key), move.PlayerUsername, string.Join(" → ", chain), move.PilePosition);
        }

        return label;
    }

    private void RefreshExitLabels()
    {
        OnPropertyChanged(nameof(WasExpelledByVote));
        OnPropertyChanged(nameof(ExpelledByVoteText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(ExitConfirmTitleText));
        OnPropertyChanged(nameof(ExitConfirmBodyText));
    }

    private void RequestExit()
    {
        ExitErrorMessage = string.Empty;
        IsExitConfirmationOpen = true;
    }

    private void CancelExit()
    {
        if (!_isLeavingMatch)
        {
            IsExitConfirmationOpen = false;
        }
    }

    private bool CanConfirmExit()
    {
        return IsExitConfirmationOpen && !_isLeavingMatch && !_isSubmittingTurn && _matchId != Guid.Empty
            && _lobbies is not null && _navigation is not null;
    }

    private async Task ConfirmExitAsync()
    {
        SessionSnapshot? session = _session?.GetActiveSession();
        if (CanConfirmExit() && session is not null && _guard?.HasActiveSession() == true && _lobbies is not null)
        {
            BoardRequestIdentity identity = new(session, _matchId, _generation);
            MatchLeaveRequest request = new()
            {
                MatchId = _matchId,
                LobbyCode = _lobbyCode,
                SessionToken = session.Token
            };
            ExitErrorMessage = string.Empty;
            SetLeavingMatch(true);
            try
            {
                bool left = await Task.Run(() => _lobbies.LeaveMatch(request));
                await Dispatcher.UIThread.InvokeAsync(() => ApplyExitResult(identity, left));
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() => ShowExitError(identity, ex));
            }
            finally
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (identity.Generation == _generation)
                    {
                        SetLeavingMatch(false);
                    }
                });
            }
        }
    }

    private void ApplyExitResult(BoardRequestIdentity identity, bool left)
    {
        if (IsCurrent(identity.Session, identity.MatchId, identity.Generation))
        {
            if (left)
            {
                ReturnToMenu();
            }
            else
            {
                ExitErrorMessage = _text.Get("Match.Exit.Failed");
            }
        }
    }

    private void ReturnToMenu()
    {
        try
        {
            _lobbies?.Disconnect();
        }
        finally
        {
            _navigation?.NavigateTo(NavigationPage.Home);
        }
    }

    private void ShowExitError(BoardRequestIdentity identity, Exception exception)
    {
        if (IsCurrent(identity.Session, identity.MatchId, identity.Generation))
        {
            ExitErrorMessage = _errors?.FromException(exception, "Match.Exit.Failed") ?? _text.Get("Match.Exit.Failed");
            if (_errors?.IsUnauthorized(exception) == true)
            {
                _guard?.Invalidate(identity.Session);
            }
        }
    }

    private void SetLeavingMatch(bool value)
    {
        _isLeavingMatch = value;
        CancelExitCommand.NotifyCanExecuteChanged();
        RefreshTurnCommands();
    }
}
