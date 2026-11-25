using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Resources;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.ViewModels.Presentation;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Contracts.Models;
using Vertigo.Client.Networking.Results;

namespace Vertigo.Client.UI.ViewModels;

public sealed class RankingViewModel : ObservableObject, INavigationAware
{
    private readonly IStatisticsClient _statistics;
    private readonly PlayerSession _session;
    private readonly UiNavigationContext _ui;
    private int _generation;
    private bool _active;
    private int _scopeIndex;
    private RankingRow? _selectedRow;
    private IReadOnlyList<RankingRow> _rows = Array.Empty<RankingRow>();
    private string _status = string.Empty;

    public RankingViewModel(IStatisticsClient statistics, PlayerSession session, UiNavigationContext ui,
        ProfileNavigation profileNavigation)
    {
        _statistics = statistics;
        _session = session;
        _ui = ui;
        BackCommand = new RelayCommand(() => ui.Navigation.NavigateTo(NavigationPage.Home));
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
        ViewProfileCommand = new RelayCommand(() => profileNavigation.Open(SelectedRow?.PlayerId));
    }

    public IRelayCommand BackCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IRelayCommand ViewProfileCommand { get; }
    public string ViewProfileText => _ui.Services.Text.Get("Profile.View");
    public string ScopeText => _ui.Services.Text.Get("Ranking.Scope");
    public bool HasScopes => _session.GetActiveSession() is { IsGuest: false };
    public IReadOnlyList<string> ScopeLabels =>
        [_ui.Services.Text.Get("Ranking.Global"), _ui.Services.Text.Get("Ranking.Friends"),
            _ui.Services.Text.Get("Ranking.Country")];

    public int ScopeIndex
    {
        get => _scopeIndex;
        set
        {
            if (value >= 0 && SetProperty(ref _scopeIndex, value) && _active)
            {
                _ = LoadAsync();
            }
        }
    }

    public RankingRow? SelectedRow
    {
        get => _selectedRow;
        set
        {
            if (SetProperty(ref _selectedRow, value))
            {
                OnPropertyChanged(nameof(CanViewProfile));
            }
        }
    }

    public bool CanViewProfile => (SelectedRow?.PlayerId ?? Guid.Empty) != Guid.Empty;

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
        OnPropertyChanged(nameof(ViewProfileText));
        OnPropertyChanged(nameof(ScopeText));
        OnPropertyChanged(nameof(ScopeLabels));
        OnPropertyChanged(nameof(HasScopes));
        _active = true;
        _ = LoadAsync();
    }

    public void OnNavigatedFrom()
    {
        _active = false;
        _generation++;
        SelectedRow = null;
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
            SelectedRow = null;
            Rows = Array.Empty<RankingRow>();
            RankingScope scope = session.IsGuest ? RankingScope.Global : (RankingScope)ScopeIndex;
            ClientResult<List<RankingEntry>> result = await Task.Run(() => ReadScope(scope, session.Token));
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
                        rows.Add(new RankingRow(rows.Count + 1, entry.PlayerId, entry.Username, entry.MatchesWon,
                            entry.FloorsBuilt, AvatarCatalog.Resolve(entry.AvatarId)));
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
    private ClientResult<List<RankingEntry>> ReadScope(RankingScope scope, string token)
    {
        return scope switch
        {
            RankingScope.Friends => _statistics.GetFriendsRanking(token),
            RankingScope.Country => _statistics.GetCountryRanking(token),
            _ => _statistics.GetRanking(token)
        };
    }
}
