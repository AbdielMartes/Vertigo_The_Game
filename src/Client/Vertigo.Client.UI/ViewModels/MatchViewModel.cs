using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Contracts.Models;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Dependencies;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MatchViewModel : ViewModelBase, INavigationAware
{
    private readonly ITextLocalizer _text;
    private readonly ILobbyClient? _lobbies;
    private readonly PlayerSession? _session;
    private readonly ISessionGuard? _guard;
    private readonly IClientErrorText? _errors;
    private readonly MatchBoardRevision _boardRevision = new();
    private int _generation;
    private bool _isLoadingBoard;
    private bool _isExitConfirmationOpen;
    private string _draftMessage = string.Empty;
    private string _roundLabel = "—";
    private string _currentTurnLabel = "—";
    private string _errorMessage = string.Empty;
    private MatchCardRow? _selectedCard;
    private MatchPileRow? _selectedPile;
    private long _currentBoardRevision;
    private bool _isPlacingCard;
    private bool _pendingBoardRefresh;
    private long _notifiedRevision;
    private IReadOnlyList<MatchBoardMove> _recentMoves = Array.Empty<MatchBoardMove>();
    private IReadOnlyList<MatchRoundSummary> _roundSummaries = Array.Empty<MatchRoundSummary>();
    private Guid _matchId;
    private string _lobbyCode = string.Empty;

    public MatchViewModel(ITextLocalizer text)
    {
        _text = text;
        RefreshBoardCommand = new AsyncRelayCommand(RefreshBoardAsync);
        PlaceCardCommand = new AsyncRelayCommand(PlaceCardAsync);
        RequestExitCommand = new RelayCommand(RequestExit);
        CancelExitCommand = new RelayCommand(CancelExit);
    }

    public MatchViewModel(MatchBoardContext context) : this(context.Text)
    {
        _lobbies = context.Lobbies;
        _session = context.Session;
        _guard = context.Guard;
        _errors = context.Errors;
    }

    public IAsyncRelayCommand RefreshBoardCommand { get; }
    public IAsyncRelayCommand PlaceCardCommand { get; }
    public MatchPileRow? SelectedPile
    {
        get => _selectedPile;
        set => SetProperty(ref _selectedPile, value);
    }
    public bool IsLoadingBoard
    {
        get => _isLoadingBoard;
        private set => SetProperty(ref _isLoadingBoard, value);
    }

    public ObservableCollection<MatchMoveRow> RecentMoves { get; } = new();

    public IReadOnlyList<MatchRoundSummary> RoundSummaries
    {
        get => _roundSummaries;
        private set => SetProperty(ref _roundSummaries, value);
    }

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
        Players.Clear();
        Piles.Clear();
        Hand.Clear();
        Wildcards.Clear();
        ChatMessages.Clear();
        DraftMessage = string.Empty;
        SelectedCard = null;
        SelectedPile = null;
        _currentBoardRevision = 0;
        _pendingBoardRefresh = false;
        _notifiedRevision = 0;
        _recentMoves = Array.Empty<MatchBoardMove>();
        RecentMoves.Clear();
        RoundSummaries = Array.Empty<MatchRoundSummary>();
        RoundLabel = "—";
        CurrentTurnLabel = "—";
        ErrorMessage = string.Empty;
        IsExitConfirmationOpen = false;
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
        }
        RefreshLocalization();
        _ = RefreshBoardAsync();
    }

    public void OnNavigatedFrom()
    {
        if (_lobbies is not null)
        {
            _lobbies.MatchBoardUpdated -= OnMatchBoardUpdated;
        }
        _generation++;
        _pendingBoardRefresh = false;
        IsExitConfirmationOpen = false;
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
                    change, _lobbyCode, _matchId, Math.Max(_currentBoardRevision, _notifiedRevision)))
            {
                _notifiedRevision = Math.Max(_notifiedRevision, change.Revision);
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
    }

    public async Task RefreshBoardAsync()
    {
        SessionSnapshot? original = _session?.GetActiveSession();
        Guid matchId = _matchId;
        string lobbyCode = _lobbyCode;
        int generation = _generation;
        if (!IsLoadingBoard && matchId != Guid.Empty && _guard is not null && _lobbies is not null
            && _guard.HasActiveSession() && original is not null)
        {
            BoardRequestIdentity request = new(original, matchId, generation);
            IsLoadingBoard = true;
            try
            {
                (MatchBoardSnapshot board, MatchPrivateHand hand) state = await Task.Run(() =>
                {
                    MatchBoardSnapshot board = _lobbies!.GetMatchBoard(lobbyCode, original.Token);
                    MatchPrivateHand hand = _lobbies.GetMatchPrivateHand(lobbyCode, original.Token);
                    return (board, hand);
                });
                await Dispatcher.UIThread.InvokeAsync(() => ApplyBoardRefresh(state.board, state.hand, request));
            }
            catch (FaultException<ServiceFault> exception)
            {
                await ReportBoardFailureAsync(exception, request);
            }
            catch (TimeoutException exception)
            {
                await ReportBoardFailureAsync(exception, request);
            }
            catch (CommunicationException exception)
            {
                await ReportBoardFailureAsync(exception, request);
            }
            catch (Exception exception)
            {
                await ReportBoardFailureAsync(exception, request);
            }
            finally
            {
                await Dispatcher.UIThread.InvokeAsync(() => CompleteBoardRefresh(request));
            }
        }
    }

    private void ApplyBoardRefresh(MatchBoardSnapshot board, MatchPrivateHand hand, BoardRequestIdentity request)
    {
        if (IsCurrent(request.Session, request.MatchId, request.Generation))
        {
            if (board.MatchId == request.MatchId && hand.MatchId == request.MatchId
                && hand.PlayerId == request.Session.PlayerId)
            {
                ApplyBoard(board);
                ErrorMessage = string.Empty;
                ReplacePrivateHand(hand);
            }
            else
            {
                ErrorMessage = _text.Get("Error.Unknown");
            }
        }
    }

    private void CompleteBoardRefresh(BoardRequestIdentity request)
    {
        if (request.Generation == _generation)
        {
            IsLoadingBoard = false;
            bool needsRefresh = _pendingBoardRefresh && _notifiedRevision > _currentBoardRevision;
            _pendingBoardRefresh = false;
            if (needsRefresh && IsCurrent(request.Session, request.MatchId, request.Generation))
            {
                _ = RefreshBoardAsync();
            }
        }
    }

    private async Task ReportBoardFailureAsync(Exception exception, BoardRequestIdentity request)
    {
        Debug.WriteLine($"Match operation failed: {exception.GetType().Name} (0x{exception.HResult:X8}).");
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (IsCurrent(request.Session, request.MatchId, request.Generation))
            {
                ErrorMessage = _errors?.FromException(exception, "Error.Unknown")
                    ?? _text.Get("Error.Unknown");
                if (_errors?.IsUnauthorized(exception) == true)
                {
                    _guard?.Invalidate(request.Session);
                }
            }
        });
    }

    public bool ApplyBoard(MatchBoardSnapshot board)
    {
        bool accepted = _boardRevision.TryAdvance(board, _matchId);
        if (accepted)
        {
            Players.Clear();
            foreach (MatchBoardPlayer player in board.Players)
            {
                Players.Add(new MatchPlayerRow
                {
                    Username = player.Username,
                    FloorsBuilt = player.FloorsBuilt,
                    IsCurrentTurn = player.PlayerId == board.CurrentTurnPlayerId,
                    IsMe = player.PlayerId == _session?.GetActiveSession()?.PlayerId
                });
            }

            _currentBoardRevision = board.Revision;
            Piles.Clear();
            SelectedPile = null;
            foreach (MatchBoardPile pile in board.Piles)
            {
                Piles.Add(new MatchPileRow
                {
                    Label = pile.Position.ToString(),
                    Count = pile.CardCount,
                    TopCardText = pile.TopCardNumber?.ToString() ?? string.Empty
                });
            }

            if (RoundSummaries.Count != board.RoundSummaries.Count)
            {
                RoundSummaries = board.RoundSummaries;
            }

            _recentMoves = board.RecentMoves;
            RefreshMoveHistory();
            RoundLabel = board.RoundNumber?.ToString() ?? "—";
            MatchBoardPlayer? current = board.Players.Find(player =>
                player.PlayerId == board.CurrentTurnPlayerId);
            CurrentTurnLabel = current?.Username ?? "—";
            ErrorMessage = string.Empty;
        }

        return accepted;
    }

    private async Task PlaceCardAsync()
    {
        SessionSnapshot? original = _session?.GetActiveSession();
        int generation = _generation;
        Guid matchId = _matchId;
        MatchCardRow? card = SelectedCard;
        MatchPileRow? pile = SelectedPile;
        if (_isPlacingCard || IsLoadingBoard || original is null || card is null || pile is null
            || !int.TryParse(card.DisplayValue, out int cardNumber)
            || !int.TryParse(pile.Label, out int pilePosition)
            || _lobbies is null || _guard is null || !_guard.HasActiveSession())
        {
            return;
        }

        _isPlacingCard = true;
        ErrorMessage = string.Empty;
        MatchPlaceRequest request = new()
        {
            LobbyCode = _lobbyCode,
            SessionToken = original.Token,
            CardNumber = cardNumber,
            PilePosition = pilePosition,
            ExpectedRevision = _currentBoardRevision
        };
        try
        {
            MatchBoardSnapshot board = await Task.Run(() => _lobbies.PlaceMatchCard(request));
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (IsCurrent(original, matchId, generation))
                {
                    if (ApplyBoard(board) && board.Revision >= _notifiedRevision)
                    {
                        _notifiedRevision = board.Revision;
                    }
                }
            });
            // Read the hand only after the accepted move, never optimistically remove cards.
            if (IsCurrent(original, matchId, generation))
            {
                await RefreshPrivateHandAsync(original, matchId, generation);
            }
        }
        catch (Exception exception)
        {
            await ReportBoardFailureAsync(exception, new BoardRequestIdentity(original, matchId, generation));
        }
        finally
        {
            _isPlacingCard = false;
        }
    }

    private async Task RefreshPrivateHandAsync(SessionSnapshot session, Guid matchId, int generation)
    {
        MatchPrivateHand hand = await Task.Run(() => _lobbies!.GetMatchPrivateHand(_lobbyCode, session.Token));
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (IsCurrent(session, matchId, generation) && hand.MatchId == matchId
                && hand.PlayerId == session.PlayerId)
            {
                ReplacePrivateHand(hand, clearSelection: true);
            }
        });
    }

    private void ReplacePrivateHand(MatchPrivateHand hand, bool clearSelection = false)
    {
        Hand.Clear();
        if (clearSelection)
        {
            SelectedCard = null;
        }

        foreach (int card in hand.Cards)
        {
            Hand.Add(new MatchCardRow { DisplayValue = card.ToString(), IsWildcard = false });
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
    public ObservableCollection<MatchCardRow> Wildcards { get; } = new();
    public ObservableCollection<MatchChatRow> ChatMessages { get; } = new();

    public string RoundLabel
    {
        get => _roundLabel;
        set => SetProperty(ref _roundLabel, value);
    }

    public string CurrentTurnLabel
    {
        get => _currentTurnLabel;
        set => SetProperty(ref _currentTurnLabel, value);
    }

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

    public string DraftMessage
    {
        get => _draftMessage;
        set
        {
            string nextValue = value ?? string.Empty;
            if (SetProperty(ref _draftMessage, nextValue))
            {
                OnPropertyChanged(nameof(ChatLengthLabel));
                OnPropertyChanged(nameof(CanSendChat));
            }
        }
    }

    public bool CanSendChat => MatchChatRules.CanSend(DraftMessage);
    public string ChatLengthLabel => $"{DraftMessage.Length}/{MatchChatRules.MaximumLength}";

    public MatchCardRow? SelectedCard
    {
        get => _selectedCard;
        set => SetProperty(ref _selectedCard, value);
    }

    public bool IsExitConfirmationOpen
    {
        get => _isExitConfirmationOpen;
        private set => SetProperty(ref _isExitConfirmationOpen, value);
    }

    public string TitleText => _text.Get("Match.Title");
    public string RoundText => _text.Get("Match.Round");
    public string TurnText => _text.Get("Match.Turn");
    public string PlayersText => _text.Get("Match.Players");
    public string FloorsText => _text.Get("Match.Floors");
    public string BoardText => _text.Get("Match.Board");
    public string BoardHintText => _text.Get("Match.BoardHint");
    public string HandText => _text.Get("Match.Hand");
    public string WildcardText => _text.Get("Match.Wildcards");
    public string RecentMovesText => _text.Get("Match.RecentMoves");
    public string ChatText => _text.Get("Match.Chat");
    public string MessageHintText => _text.Get("Match.MessageHint");
    public string SendText => _text.Get("Match.Send");
    public string PlayCardText => _text.Get("Match.PlayCard");
    public string ExitText => _text.Get("Match.Exit");
    public string ExitConfirmTitleText => _text.Get("Match.ExitConfirmTitle");
    public string ExitConfirmBodyText => _text.Get("Match.ExitConfirmBody");
    public string CancelText => _text.Get("Match.Cancel");
    public string ConfirmText => _text.Get("Match.Confirm");
    public string ErrorTitleText => _text.Get("Room.ErrorHeading");

    public IRelayCommand RequestExitCommand { get; }
    public IRelayCommand CancelExitCommand { get; }

    public void RefreshLocalization()
    {
        OnPropertyChanged(nameof(TitleText));
        OnPropertyChanged(nameof(RoundText));
        OnPropertyChanged(nameof(TurnText));
        OnPropertyChanged(nameof(PlayersText));
        OnPropertyChanged(nameof(FloorsText));
        OnPropertyChanged(nameof(BoardText));
        OnPropertyChanged(nameof(BoardHintText));
        OnPropertyChanged(nameof(HandText));
        OnPropertyChanged(nameof(WildcardText));
        OnPropertyChanged(nameof(ChatText));
        OnPropertyChanged(nameof(RecentMovesText));
        RefreshMoveHistory();
        OnPropertyChanged(nameof(MessageHintText));
        OnPropertyChanged(nameof(SendText));
        OnPropertyChanged(nameof(PlayCardText));
        OnPropertyChanged(nameof(ExitText));
        OnPropertyChanged(nameof(ExitConfirmTitleText));
        OnPropertyChanged(nameof(ExitConfirmBodyText));
        OnPropertyChanged(nameof(CancelText));
        OnPropertyChanged(nameof(ConfirmText));
        OnPropertyChanged(nameof(ErrorTitleText));
    }

    private void RefreshMoveHistory()
    {
        RecentMoves.Clear();
        foreach (MatchBoardMove move in _recentMoves.Reverse())
        {
            RecentMoves.Add(new MatchMoveRow(FormatMove(move)));
        }
    }

    private string FormatMove(MatchBoardMove move)
    {
        string label;
        if (move.ActionKind == MatchBoardActionKind.Dismantle)
        {
            label = string.Format(_text.Get("Match.Move.Dismantle"), move.PlayerUsername, move.PilePosition);
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

    private void RequestExit()
    {
        IsExitConfirmationOpen = true;
    }

    private void CancelExit()
    {
        IsExitConfirmationOpen = false;
    }

}
