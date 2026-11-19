using System;
using System.Collections.Generic;
using System.Linq;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MatchResultsViewModel : ViewModelBase
{
    private readonly ITextLocalizer _text;
    private IReadOnlyList<MatchRoundSummary> _roundSummaries = Array.Empty<MatchRoundSummary>();
    private IReadOnlyList<MatchRoundResultRow> _rounds = Array.Empty<MatchRoundResultRow>();
    private MatchRoundResultRow? _selectedRound;
    private MatchEndReason _endReason;
    private IReadOnlyList<string> _winnerNames = Array.Empty<string>();
    private bool _isExpanded;

    internal MatchResultsViewModel(ITextLocalizer text)
    {
        _text = text;
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
                OnPropertyChanged(nameof(SelectedScores));
            }
        }
    }

    public IReadOnlyList<MatchRoundPlayerScore> SelectedScores =>
        SelectedRound?.Scores ?? Array.Empty<MatchRoundPlayerScore>();

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    public bool HasRoundSummaries => _roundSummaries.Count > 0;
    public bool HasResults => HasRoundSummaries || HasFinalResult;
    public bool HasFinalResult => _endReason != MatchEndReason.None;
    public bool IsAbandonment => _endReason == MatchEndReason.Abandonment;
    public string HeaderText => HasFinalResult || !HasRoundSummaries ? _text.Get("Match.Results.Title")
        : string.Format(_text.Get("Match.Results.RoundClosed"), _roundSummaries[^1].RoundNumber);
    public string WinnerText => _winnerNames.Count switch
    {
        0 => string.Empty,
        1 => string.Format(_text.Get("Match.Results.Winner"), _winnerNames[0]),
        _ => string.Format(_text.Get("Match.Results.Winners"), string.Join(", ", _winnerNames))
    };

    public string AbandonmentText => _text.Get("Match.Results.Abandonment");
    public string SelectRoundText => _text.Get("Match.Results.SelectRound");
    public string PlayerText => _text.Get("Match.Results.Player");
    public string FloorsText => _text.Get("Match.Results.Floors");
    public string BonusText => _text.Get("Match.Results.Bonus");
    public string TotalText => _text.Get("Match.Results.Total");

    internal void ApplyBoard(MatchBoardSnapshot board)
    {
        if (_roundSummaries.Count != board.RoundSummaries.Count)
        {
            int? selectedRound = SelectedRound?.RoundNumber;
            bool reviewingEarlierRound = _roundSummaries.Count > 0 && selectedRound.HasValue
                && selectedRound.Value != _roundSummaries[^1].RoundNumber;
            _roundSummaries = board.RoundSummaries;
            BuildRoundOptions(reviewingEarlierRound ? selectedRound : null);
            OnPropertyChanged(nameof(HasRoundSummaries));
        }

        if (_endReason != board.EndReason)
        {
            bool endedNow = !HasFinalResult && board.EndReason != MatchEndReason.None;
            _endReason = board.EndReason;
            Dictionary<Guid, string> names = board.Players.ToDictionary(
                player => player.PlayerId, player => player.Username);
            _winnerNames = board.WinnerIds.Select(id => names.GetValueOrDefault(id))
                .Where(name => !string.IsNullOrWhiteSpace(name)).Select(name => name!).ToArray();
            OnPropertyChanged(nameof(HasFinalResult));
            OnPropertyChanged(nameof(IsAbandonment));
            OnPropertyChanged(nameof(WinnerText));
            if (endedNow)
            {
                IsExpanded = true;
            }
        }

        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(HeaderText));
    }

    internal void Reset()
    {
        _roundSummaries = Array.Empty<MatchRoundSummary>();
        BuildRoundOptions(null);
        _endReason = MatchEndReason.None;
        _winnerNames = Array.Empty<string>();
        IsExpanded = false;
        OnPropertyChanged(nameof(HasRoundSummaries));
        OnPropertyChanged(nameof(HasFinalResult));
        OnPropertyChanged(nameof(IsAbandonment));
        OnPropertyChanged(nameof(HasResults));
        OnPropertyChanged(nameof(HeaderText));
        OnPropertyChanged(nameof(WinnerText));
    }

    internal void RefreshLocalization()
    {
        BuildRoundOptions(SelectedRound?.RoundNumber);
        OnPropertyChanged(nameof(HeaderText));
        OnPropertyChanged(nameof(WinnerText));
        OnPropertyChanged(nameof(AbandonmentText));
        OnPropertyChanged(nameof(SelectRoundText));
        OnPropertyChanged(nameof(PlayerText));
        OnPropertyChanged(nameof(FloorsText));
        OnPropertyChanged(nameof(BonusText));
        OnPropertyChanged(nameof(TotalText));
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
