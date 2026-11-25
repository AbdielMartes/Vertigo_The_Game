using System;
using System.Collections.Generic;
using System.Linq;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MatchResultsViewModel : ViewModelBase
{
    private static readonly TimeSpan RoundPreviewDuration = TimeSpan.FromSeconds(7);
    private readonly ITextLocalizer _text;
    private readonly TimeProvider _clock;
    private long? _roundPreviewStarted;
    private IReadOnlyList<MatchRoundSummary> _roundSummaries = Array.Empty<MatchRoundSummary>();
    private IReadOnlyList<MatchRoundResultRow> _rounds = Array.Empty<MatchRoundResultRow>();
    private MatchRoundResultRow? _selectedRound;
    private Guid? _localPlayerId;
    private MatchTowerResult? _selectedTower;
    private MatchTowerResult? _latestTower;
    private MatchTowerResult? _finalTower;
    private IReadOnlyList<MatchFinalStandingRow> _finalStandings = Array.Empty<MatchFinalStandingRow>();
    private MatchEndReason _endReason;
    private IReadOnlyList<string> _winnerNames = Array.Empty<string>();
    private bool _isExpanded;

    internal MatchResultsViewModel(ITextLocalizer text, TimeProvider? clock = null)
    {
        _text = text;
        _clock = clock ?? TimeProvider.System;
    }

    public IReadOnlyList<MatchRoundSummary> RoundSummaries => _roundSummaries;

    public IReadOnlyList<MatchRoundResultRow> Rounds
    {
        get => _rounds;
        private set => SetProperty(ref _rounds, value);
    }

    public MatchRoundResultRow? SelectedRound
    {
        get => _selectedRound;
        set
        {
            if (SetProperty(ref _selectedRound, value))
            {
                _selectedTower = BuildRoundTower(SelectedScores);
                OnPropertyChanged(nameof(SelectedScores));
                OnPropertyChanged(nameof(SelectedTower));
                OnPropertyChanged(nameof(HasSelectedTower));
                OnPropertyChanged(nameof(SelectedTowerCaption));
            }
        }
    }

    public IReadOnlyList<MatchRoundPlayerScore> SelectedScores =>
        SelectedRound?.Scores ?? Array.Empty<MatchRoundPlayerScore>();

    public MatchTowerResult? SelectedTower => _selectedTower;
    public bool HasSelectedTower => SelectedTower is not null;
    public string SelectedTowerCaption => FormatTower(SelectedTower);

    public MatchTowerResult? LatestTower => _latestTower;
    public bool ShowLatestTower => !HasFinalResult && LatestTower is not null;
    public string LatestTowerCaption => FormatTower(LatestTower);

    public MatchTowerResult? FinalTower => _finalTower;
    public bool HasFinalTower => _finalTower is not null;
    public string FinalTowerCaption => FormatTower(_finalTower);
    public IReadOnlyList<MatchFinalStandingRow> FinalStandings => _finalStandings;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (SetProperty(ref _isExpanded, value))
            {
                _roundPreviewStarted = null;
                OnPropertyChanged(nameof(IsRoundSummaryOpen));
            }
        }
    }

    public bool HasRoundSummaries => _roundSummaries.Count > 0;
    public bool ShowRoundSummary => HasRoundSummaries && !HasFinalResult;
    public bool IsRoundSummaryOpen => ShowRoundSummary && IsExpanded;
    public bool HasResults => HasRoundSummaries || HasFinalResult;
    public bool HasFinalResult => _endReason != MatchEndReason.None;
    public bool IsAbandonment => _endReason == MatchEndReason.Abandonment;
    public bool IsEarlyAbandonment => IsAbandonment && !HasRoundSummaries;
    public bool IsLocalWinner => _finalStandings.Any(row => row.IsLocal && row.IsWinner);
    public bool IsLocalDefeat => !IsAbandonment && _finalStandings.Any(row => row.IsWinner)
        && _finalStandings.Any(row => row.IsLocal && row.Position.HasValue && !row.IsWinner);
    public string HeaderText => HasFinalResult || !HasRoundSummaries ? _text.Get("Match.Results.Title")
        : string.Format(_text.Get("Match.Results.RoundClosed"), _roundSummaries[^1].RoundNumber);
    public string WinnerText => _winnerNames.Count switch
    {
        0 => string.Empty,
        1 => string.Format(_text.Get("Match.Results.Winner"), _winnerNames[0]),
        _ => string.Format(_text.Get("Match.Results.Winners"), string.Join(", ", _winnerNames))
    };

    public string ResultTitle => _text.Get(GetResultTitleKey());
    public string ResultDetail => IsEarlyAbandonment ? _text.Get("Match.Results.EarlyAbandonmentDetail") : WinnerText;
    public string LocalPlayerText => _text.Get("Match.Results.You");
    public string WinnerIconText => _text.Get("Match.Results.WinnerIcon");
    public string PersonalTowerText => _text.Get("Match.Results.PersonalTower");
    public string FinalTowerText => _text.Get("Match.Results.FinalTower");
    public string StandingsText => _text.Get(IsEarlyAbandonment ? "Match.Players" : "Match.Results.Standings");
    public string SelectRoundText => _text.Get("Match.Results.SelectRound");
    public string PlayerText => _text.Get("Match.Results.Player");
    public string FloorsText => _text.Get("Match.Results.Floors");
    public string BonusText => _text.Get("Match.Results.Bonus");
    public string TotalText => _text.Get("Match.Results.Total");

    internal void ApplyBoard(MatchBoardSnapshot board, Guid? localPlayerId = null)
    {
        bool newRound = board.RoundSummaries.Count > _roundSummaries.Count;
        bool identityChanged = _localPlayerId != localPlayerId;
        _localPlayerId = localPlayerId;
        if (identityChanged || _roundSummaries.Count != board.RoundSummaries.Count)
        {
            UpdateRoundSummaries(board.RoundSummaries);
        }

        if (_endReason != board.EndReason || (identityChanged && HasFinalResult))
        {
            bool endedNow = !HasFinalResult && board.EndReason != MatchEndReason.None;
            _endReason = board.EndReason;
            ApplyFinalResult(board);
            if (endedNow)
            {
                IsExpanded = true;
                _roundPreviewStarted = null;
            }
        }

        if (newRound && !HasFinalResult)
        {
            if (!IsExpanded)
            {
                IsExpanded = true;
                _roundPreviewStarted = _clock.GetTimestamp();
            }
            else if (_roundPreviewStarted.HasValue)
            {
                _roundPreviewStarted = _clock.GetTimestamp();
            }
        }

        OnPropertyChanged(nameof(ShowRoundSummary));
        OnPropertyChanged(nameof(IsRoundSummaryOpen));
        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(HeaderText));
    }

    public void ToggleRoundSummary()
    {
        if (ShowRoundSummary)
        {
            IsExpanded = !IsExpanded;
        }
    }

    internal void UpdateRoundPreview()
    {
        if (_roundPreviewStarted is long started && _clock.GetElapsedTime(started) >= RoundPreviewDuration)
        {
            IsExpanded = false;
        }
    }

    internal void Reset()
    {
        _localPlayerId = null;
        _roundSummaries = Array.Empty<MatchRoundSummary>();
        BuildRoundOptions(null);
        _endReason = MatchEndReason.None;
        _winnerNames = Array.Empty<string>();
        _latestTower = null;
        _finalTower = null;
        _finalStandings = Array.Empty<MatchFinalStandingRow>();
        IsExpanded = false;
        OnPropertyChanged(nameof(HasRoundSummaries));
        OnPropertyChanged(nameof(ShowRoundSummary));
        OnPropertyChanged(nameof(IsRoundSummaryOpen));
        RefreshFinalResult();
        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(HeaderText));
    }

    internal void RefreshLocalization()
    {
        BuildRoundOptions(SelectedRound?.RoundNumber);
        OnPropertyChanged(nameof(HeaderText));
        OnPropertyChanged(nameof(WinnerText));
        OnPropertyChanged(nameof(ResultTitle));
        OnPropertyChanged(nameof(ResultDetail));
        OnPropertyChanged(nameof(LocalPlayerText));
        OnPropertyChanged(nameof(WinnerIconText));
        OnPropertyChanged(nameof(PersonalTowerText));
        OnPropertyChanged(nameof(FinalTowerText));
        OnPropertyChanged(nameof(StandingsText));
        OnPropertyChanged(nameof(FinalTowerCaption));
        RefreshLatestTower();
        OnPropertyChanged(nameof(SelectRoundText));
        OnPropertyChanged(nameof(PlayerText));
        OnPropertyChanged(nameof(FloorsText));
        OnPropertyChanged(nameof(BonusText));
        OnPropertyChanged(nameof(TotalText));
    }

    private void UpdateRoundSummaries(IReadOnlyList<MatchRoundSummary> summaries)
    {
        int? selectedRound = SelectedRound?.RoundNumber;
        bool reviewingEarlierRound = _roundSummaries.Count > 0 && selectedRound.HasValue
            && selectedRound.Value != _roundSummaries[^1].RoundNumber;
        _roundSummaries = summaries;
        _latestTower = _roundSummaries.Count == 0 ? null : BuildRoundTower(_roundSummaries[^1].Players);
        BuildRoundOptions(reviewingEarlierRound ? selectedRound : null);
        OnPropertyChanged(nameof(HasRoundSummaries));
        RefreshLatestTower();
    }

    private MatchTowerResult? BuildRoundTower(IReadOnlyList<MatchRoundPlayerScore> scores)
    {
        MatchRoundPlayerScore? personal = scores.FirstOrDefault(score => score.PlayerId == _localPlayerId);
        MatchTowerResult? result = null;
        if (personal is not null)
        {
            int tallest = scores.Max(score => score.FloorsBuilt);
            result = new MatchTowerResult((personal.FloorsBuilt, personal.BonusFloors, personal.TotalFloors),
                tallest, includeBonusInDrawing: false);
        }

        return result;
    }

    private string FormatTower(MatchTowerResult? tower)
    {
        return tower is null ? string.Empty : string.Format(_text.Get("Match.Results.TowerCaption"),
            tower.BuiltFloors, tower.BonusFloors, tower.TotalFloors);
    }

    private void ApplyFinalResult(MatchBoardSnapshot board)
    {
        IReadOnlyList<Guid> winners = IsEarlyAbandonment ? Array.Empty<Guid>() : board.WinnerIds;
        Dictionary<Guid, string> names = board.Players.ToDictionary(
            player => player.PlayerId, player => player.Username);
        _winnerNames = winners.Select(id => names.GetValueOrDefault(id))
            .Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name!).ToArray();
        _finalStandings = board.Players
            .Select(player => new MatchFinalStandingRow
            {
                Position = IsEarlyAbandonment ? null : player.FinalPosition,
                Username = player.Username,
                TotalFloors = player.GlobalFloors,
                IsWinner = winners.Contains(player.PlayerId),
                IsLocal = player.PlayerId == _localPlayerId
            })
            .OrderBy(row => row.Position ?? int.MaxValue)
            .ThenByDescending(row => row.IsWinner)
            .ThenByDescending(row => row.TotalFloors)
            .ToArray();
        _finalTower = IsEarlyAbandonment ? null : BuildFinalTower(board);
        RefreshFinalResult();
    }

    private string GetResultTitleKey()
    {
        string key = "Match.Results.Finished";
        if (IsEarlyAbandonment)
        {
            key = "Match.Results.NoWinner";
        }
        else if (IsAbandonment)
        {
            key = IsLocalWinner ? "Match.Results.Abandonment" : "Match.Results.FinishedByAbandonment";
        }
        else if (IsLocalWinner)
        {
            key = _finalStandings.Count(row => row.IsWinner) > 1 ? "Match.Results.SharedVictory" : "Match.Results.Victory";
        }
        else if (IsLocalDefeat)
        {
            key = "Match.Results.Defeat";
        }

        return key;
    }

    private MatchTowerResult? BuildFinalTower(MatchBoardSnapshot board)
    {
        MatchBoardPlayer? personal = board.Players.Find(player => player.PlayerId == _localPlayerId);
        MatchTowerResult? result = null;
        if (personal is not null)
        {
            MatchRoundPlayerScore[] scores = _roundSummaries.SelectMany(round => round.Players)
                .Where(score => score.PlayerId == personal.PlayerId).ToArray();
            int built = scores.Sum(score => score.FloorsBuilt);
            int bonus = scores.Sum(score => score.BonusFloors);
            result = built + bonus == personal.GlobalFloors
                ? new MatchTowerResult((built, bonus, personal.GlobalFloors),
                    board.Players.Max(player => player.GlobalFloors), includeBonusInDrawing: true)
                : null;
        }

        return result;
    }

    private void RefreshLatestTower()
    {
        OnPropertyChanged(nameof(LatestTower));
        OnPropertyChanged(nameof(LatestTowerCaption));
        OnPropertyChanged(nameof(ShowLatestTower));
    }

    private void RefreshFinalResult()
    {
        OnPropertyChanged(nameof(HasFinalResult));
        OnPropertyChanged(nameof(IsAbandonment));
        OnPropertyChanged(nameof(IsEarlyAbandonment));
        OnPropertyChanged(nameof(IsLocalWinner));
        OnPropertyChanged(nameof(IsLocalDefeat));
        OnPropertyChanged(nameof(WinnerText));
        OnPropertyChanged(nameof(ResultTitle));
        OnPropertyChanged(nameof(ResultDetail));
        OnPropertyChanged(nameof(StandingsText));
        OnPropertyChanged(nameof(FinalTower));
        OnPropertyChanged(nameof(HasFinalTower));
        OnPropertyChanged(nameof(FinalTowerCaption));
        OnPropertyChanged(nameof(FinalStandings));
        RefreshLatestTower();
    }

    private void BuildRoundOptions(int? selectedRoundNumber)
    {
        Rounds = _roundSummaries.Reverse().Select(round => new MatchRoundResultRow(
            round.RoundNumber, string.Format(_text.Get("Match.Results.RoundOption"), round.RoundNumber),
            round.Players)).ToArray();
        SelectedRound = Rounds.FirstOrDefault(round => round.RoundNumber == selectedRoundNumber)
            ?? Rounds.FirstOrDefault();
    }
}
