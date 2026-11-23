using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Presentation;

namespace Vertigo.Client.UI.ViewModels;

public sealed class RankingViewModel : ObservableObject, INavigationAware
{
    private readonly IStatisticsClient _statistics;
    private readonly PlayerSession _session;
    private readonly UiNavigationContext _ui;
    private int _generation;
    private IReadOnlyList<RankingRow> _rows = Array.Empty<RankingRow>();
    private string _status = string.Empty;

    public RankingViewModel(IStatisticsClient statistics, PlayerSession session, UiNavigationContext ui)
    {
        _statistics = statistics;
        _session = session;
        _ui = ui;
        BackCommand = new RelayCommand(() => ui.Navigation.NavigateTo(NavigationPage.Home));
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public string Title => _ui.Services.Text.Get("Home.Ranking");
    public string BackText => _ui.Services.Text.Get("Navigation.Back");
    public string RefreshText => _ui.Services.Text.Get("Ranking.Refresh");
    public string PlayerText => _ui.Services.Text.Get("Ranking.Player");
    public string WinsText => _ui.Services.Text.Get("Ranking.Wins");
    public string FloorsText => _ui.Services.Text.Get("Ranking.Floors");

    public IReadOnlyList<RankingRow> Rows
    {
        get => _rows;
        private set => SetProperty(ref _rows, value);
    }

    public string Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    public void OnNavigatedTo()
    {
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(RefreshText));
        OnPropertyChanged(nameof(PlayerText));
        OnPropertyChanged(nameof(WinsText));
        OnPropertyChanged(nameof(FloorsText));
        _ = LoadAsync();
    }

    public void OnNavigatedFrom()
    {
        _generation++;
        Rows = Array.Empty<RankingRow>();
        Status = string.Empty;
    }

    private async Task LoadAsync()
    {
        SessionSnapshot? session = _session.GetActiveSession();
        if (session is not null)
        {
            int generation = ++_generation;
            Status = _ui.Services.Text.Get("Ranking.Loading");
            var result = await Task.Run(() => _statistics.GetRanking(session.Token));
            if (generation == _generation && _ui.Services.SessionGuard.IsCurrent(session))
            {
                if (_ui.Services.Errors.IsUnauthorized(result))
                {
                    _ui.Services.SessionGuard.Invalidate(session);
                }
                else if (result.IsSuccess && result.Value is { } entries)
                {
                    List<RankingRow> rows = new(entries.Count);
                    foreach (var entry in entries)
                    {
                        rows.Add(new RankingRow(rows.Count + 1, entry.Username, entry.MatchesWon, entry.FloorsBuilt));
                    }

                    Rows = rows;
                    Status = rows.Count == 0 ? _ui.Services.Text.Get("Ranking.Empty") : string.Empty;
                }
                else
                {
                    Status = _ui.Services.Errors.FromResult(result, "Error.Unknown");
                }
            }
        }
    }
}
