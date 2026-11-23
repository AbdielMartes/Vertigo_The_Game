using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Results;
using Vertigo.Client.Networking.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels;

public sealed class HomeViewModel : ObservableObject, INavigationAware
{
    private readonly ISessionCoordinator _sessions;
    private readonly PlayerSession _playerSession;
    private readonly INavigator _navigation;
    private readonly LobbyViewModel _lobby;
    private readonly IClientErrorText _errors;
    private readonly ISessionGuard _coordinator;
    private readonly ITextLocalizer _text;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _isNotificationsOpen;

    public HomeViewModel(HomeContext context, LobbyViewModel lobby, PlayViewModel play)
    {
        _sessions = context.Sessions;
        _playerSession = context.PlayerSession;
        _navigation = context.Ui.Navigation;
        _lobby = lobby;
        Play = play;
        _errors = context.Ui.Services.Errors;
        _coordinator = context.Ui.Services.SessionGuard;
        _text = context.Ui.Services.Text;
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        OpenSocialCommand = new RelayCommand(OpenSocial);
        OpenPlayCommand = new RelayCommand(OpenPlay);
        ToggleNotificationsCommand = new RelayCommand(ToggleNotifications);
        OpenProfileCommand = new RelayCommand(OpenProfile);
        OpenRankingCommand = new RelayCommand(OpenRanking);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
    }

    public PlayViewModel Play { get; }
    public IRelayCommand OpenPlayCommand { get; }

    public string Username => _playerSession.GetActiveSession()?.Username ?? string.Empty;
    public bool IsGuest => _playerSession.GetActiveSession()?.IsGuest ?? false;

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public IAsyncRelayCommand LogoutCommand { get; }
    public IRelayCommand OpenSocialCommand { get; }
    public IRelayCommand ToggleNotificationsCommand { get; }
    public IRelayCommand OpenProfileCommand { get; }
    public IRelayCommand OpenRankingCommand { get; }
    public IRelayCommand OpenSettingsCommand { get; }

    public bool IsNotificationsOpen
    {
        get => _isNotificationsOpen;
        private set => SetProperty(ref _isNotificationsOpen, value);
    }

    public string PlayerTag => string.IsNullOrEmpty(Username) ? string.Empty : "@" + Username;
    public string AccountKindText => _text.Get(IsGuest ? "Home.GuestAccount" : "Home.RegisteredAccount");
    public string BrandSubtitle => _text.Get("Home.BrandSubtitle");
    public string PlayText => _text.Get("Home.Play");
    public string SocialText => _text.Get("Home.Social");
    public string RankingText => _text.Get("Home.Ranking");
    public string SettingsText => _text.Get("Home.Settings");
    public string ProfileText => _text.Get("Home.Profile");
    public string NotificationsText => _text.Get("Home.Notifications");
    public string NoNotificationsText => _text.Get("Home.NotificationsHelp");
    public string OpenSocialNotificationsText => _text.Get("Home.OpenSocialNotifications");
    public string GlossaryText => _text.Get("Home.Glossary");
    public string LogoutText => _text.Get("Home.Logout");
    public string CloseText => _text.Get("Home.Close");
    public string NotAvailableText => _text.Get("Home.NotAvailable");
    public string VersionText => "v0.1.0";


    public void OnNavigatedTo()
    {
        ErrorMessage = string.Empty;
        IsNotificationsOpen = false;
        if (_coordinator.HasActiveSession())
        {
            RefreshIdentity();
            NotifyLanguageChanged();
        }
    }

    public void OnNavigatedFrom()
    {
        ErrorMessage = string.Empty;
        IsNotificationsOpen = false;
    }

    public void RefreshIdentity()
    {
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(IsGuest));
        OnPropertyChanged(nameof(PlayerTag));
        OnPropertyChanged(nameof(AccountKindText));
    }

    private void OpenPlay()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Play);
        }
    }

    private void ToggleNotifications()
    {
        IsNotificationsOpen = !IsNotificationsOpen;
    }

    private void OpenProfile()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Profile);
        }
    }

    private void OpenRanking()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Ranking);
        }
    }

    private void OpenSettings()
    {
        if (!IsBusy && _coordinator.HasActiveSession())
        {
            _navigation.NavigateTo(NavigationPage.Settings);
        }
    }

    private void NotifyLanguageChanged()
    {
        OnPropertyChanged(nameof(AccountKindText));
        OnPropertyChanged(nameof(BrandSubtitle));
        OnPropertyChanged(nameof(PlayText));
        OnPropertyChanged(nameof(SocialText));
        OnPropertyChanged(nameof(RankingText));
        OnPropertyChanged(nameof(SettingsText));
        OnPropertyChanged(nameof(ProfileText));
        OnPropertyChanged(nameof(NotificationsText));
        OnPropertyChanged(nameof(NoNotificationsText));
        OnPropertyChanged(nameof(OpenSocialNotificationsText));
        OnPropertyChanged(nameof(LogoutText));
        OnPropertyChanged(nameof(GlossaryText));
        OnPropertyChanged(nameof(CloseText));
        OnPropertyChanged(nameof(NotAvailableText));
    }

    private void OpenSocial()
    {
        SessionSnapshot? session = _playerSession.GetActiveSession();
        if (session is null)
        {
            _coordinator.Invalidate();
        }
        else if (session.IsGuest)
        {
            ErrorMessage = _text.Get("Social.RegisteredOnly");
        }
        else
        {
            _navigation.NavigateTo(NavigationPage.Social);
        }
    }

    private async Task LogoutAsync()
    {
        if (!IsBusy && string.IsNullOrEmpty(_lobby.LobbyCode) && _coordinator.HasActiveSession())
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                SessionSnapshot? session = _playerSession.GetActiveSession();
                ClientResult<bool> result = await Task.Run(_sessions.Logout);
                if (_errors.IsUnauthorized(result) || (result.IsSuccess && result.Value))
                {
                    _coordinator.Invalidate(session);
                }
                else
                {
                    ErrorMessage = _errors.FromResult(result, "Home.LogoutFailed");
                }
            }
            catch (Exception exception) { ErrorMessage = _errors.FromException(exception, "Home.LogoutFailed"); }
            finally { IsBusy = false; }
        }
    }
}
