using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class ProfileViewModel : ObservableObject, INavigationAware
{
    private readonly PlayerSession _session;
    private readonly INavigator _navigation;
    private readonly ITextLocalizer _text;

    public ProfileViewModel(PlayerSession session, INavigator navigation, ITextLocalizer text)
    {
        _session = session;
        _navigation = navigation;
        _text = text;
        BackCommand = new RelayCommand(() => _navigation.NavigateTo(NavigationPage.Home));
    }

    public IRelayCommand BackCommand { get; }
    public string HeadingText => _text.Get("Profile.Title");
    public string UsernameText => _text.Get("Profile.Username");
    public string AccountText => _text.Get("Profile.Account");
    public string BackText => _text.Get("Navigation.Back");
    public string Username => _session.GetActiveSession()?.Username ?? string.Empty;
    public string AccountType => _text.Get(_session.GetActiveSession()?.IsGuest == true
        ? "Home.GuestAccount" : "Home.RegisteredAccount");

    public void OnNavigatedTo()
    {
        OnPropertyChanged(nameof(HeadingText));
        OnPropertyChanged(nameof(UsernameText));
        OnPropertyChanged(nameof(AccountText));
        OnPropertyChanged(nameof(BackText));
        OnPropertyChanged(nameof(Username));
        OnPropertyChanged(nameof(AccountType));
    }

    public void OnNavigatedFrom()
    {
    }
}
