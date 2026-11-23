using System.Globalization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class ProfileViewModel : ObservableObject, INavigationAware
{
    private readonly PlayerSession _session;
    private readonly UiNavigationContext _ui;
    private readonly IStatisticsClient _statistics;
    private int _generation;
    private string _winRate = string.Empty;

    public ProfileViewModel(PlayerSession session, UiNavigationContext ui, IStatisticsClient statistics)
    {
        _session = session;
        _ui = ui;
        _statistics = statistics;
        BackCommand = new RelayCommand(() => _ui.Navigation.NavigateTo(NavigationPage.Home));
    }

    public IRelayCommand BackCommand { get; }
    public string HeadingText => _ui.Services.Text.Get("Profile.Title");
    public string UsernameText => _ui.Services.Text.Get("Profile.Username");
    public string AccountText => _ui.Services.Text.Get("Profile.Account");
    public string BackText => _ui.Services.Text.Get("Navigation.Back");
    public string WinRateText => _ui.Services.Text.Get("Profile.WinRate");
    public bool ShowWinRate => _session.GetActiveSession() is { IsGuest: false };
    public string Username => _session.GetActiveSession()?.Username ?? string.Empty;
    public string AccountType => _ui.Services.Text.Get(_session.GetActiveSession()?.IsGuest == true
        ? "Home.GuestAccount" : "Home.RegisteredAccount");

    public string WinRate
    {
        get => _winRate;
        private set => SetProperty(ref _winRate, value);
    }

    public void OnNavigatedTo()
    {
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(UsernameText));
        OnPropertyChanged(nameof(AccountText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(AccountType));
        OnPropertyChanged(nameof(WinRateText));
        OnPropertyChanged(nameof(ShowWinRate));
        if (_session.GetActiveSession() is { IsGuest: false } session)
        {
            WinRate = _ui.Services.Text.Get("Ranking.Loading");
            _ = LoadWinRateAsync(session, ++_generation);
        }
    }

    public void OnNavigatedFrom()
    {
        _generation++;
        WinRate = string.Empty;
    }

    private async Task LoadWinRateAsync(SessionSnapshot session, int generation)
    {
        var result = await Task.Run(() => _statistics.GetMyRecord(session.Token));
        if (generation == _generation && _ui.Services.SessionGuard.IsCurrent(session))
        {
            if (_ui.Services.Errors.IsUnauthorized(result))
            {
                _ui.Services.SessionGuard.Invalidate(session);
            }
            else if (result.IsSuccess && result.Value is { } record)
            {
                WinRate = record.MatchesPlayed == 0
                    ? _ui.Services.Text.Get("Profile.NoMatches")
                    : ((double)record.MatchesWon / record.MatchesPlayed).ToString("P0", CultureInfo.CurrentCulture);
            }
            else
            {
                WinRate = _ui.Services.Errors.FromResult(result, "Error.Unknown");
            }
        }
    }
}
