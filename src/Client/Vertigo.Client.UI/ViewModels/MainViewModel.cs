using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.ViewModels.Lifecycle;

namespace Vertigo.Client.UI.ViewModels;

public sealed class MainViewModel : ViewModelBase, IDisposable
{
    private readonly INavigator _navigation;
    private readonly PlayerSession _playerSession;
    private readonly HomeViewModel _home;
    private bool _disposed;
    private NavigationPage _previousPage;

    public MainViewModel(INavigator navigation, AccessViewModel access, MainContext context)
    {
        _navigation = navigation;
        _playerSession = context.SocialContext.PlayerSession;
        Access = access;
        Home = context.Home;
        Play = context.Home.Play;
        Lobby = context.Lobby;
        Match = context.Lobby.Match;
        Social = context.SocialContext.Social;
        Settings = context.Settings;
        Profile = context.Profile;
        _home = context.Home;
        _previousPage = _navigation.CurrentPage;
        _navigation.PageChanged += OnPageChanged;
        if (_playerSession.IsAuthenticated) _navigation.NavigateTo(NavigationPage.Home);
        else Access.OnNavigatedTo();
    }

    public AccessViewModel Access { get; }
    public HomeViewModel Home { get; }
    public PlayViewModel Play { get; }
    public LobbyViewModel Lobby { get; }
    public MatchViewModel Match { get; }
    public SocialViewModel Social { get; }
    public SettingsViewModel Settings { get; }
    public ProfileViewModel Profile { get; }

    public NavigationPage CurrentPage => _navigation.CurrentPage;
    public bool IsAccessPage => CurrentPage == NavigationPage.Access;

    public AccessViewModel? AccessContent => IsAccessPage ? Access : null;

    public object? TemplatedContent => IsAccessPage ? null : CurrentViewModel;


    public object CurrentViewModel => CurrentPage switch
    {
        NavigationPage.Home => Home,
        NavigationPage.Play => Play,
        NavigationPage.Lobby => Lobby,
        NavigationPage.Match => Match,
        NavigationPage.Social => Social,
        NavigationPage.Settings => Settings,
        NavigationPage.Profile => Profile,
        _ => Access
    };


    private void OnPageChanged(object? sender, NavigationPage page)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(() => OnPageChanged(sender, page));
            return;
        }
        if (_disposed) return;
        if (page != NavigationPage.Access && !_playerSession.IsAuthenticated)
        {
            _navigation.NavigateTo(NavigationPage.Access);
            return;
        }

        bool returnedFromMatch = _previousPage == NavigationPage.Match && page == NavigationPage.Home;
        GetPage(_previousPage)?.OnNavigatedFrom();
        _previousPage = page;
        if (returnedFromMatch)
        {
            Lobby.ResetState();
        }
        if (page == NavigationPage.Access)
        {
            Lobby.ResetState();
            Social.Reset();
        }
        GetPage(page)?.OnNavigatedTo();
        OnPropertyChanged(nameof(CurrentPage));
        OnPropertyChanged(nameof(IsAccessPage));
        OnPropertyChanged(nameof(AccessContent));
        OnPropertyChanged(nameof(TemplatedContent));
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    private INavigationAware? GetPage(NavigationPage page)
    {
        return page switch
        {
            NavigationPage.Access => Access,
            NavigationPage.Home => Home,
            NavigationPage.Play => Play,
            NavigationPage.Lobby => Lobby,
            NavigationPage.Match => Match,
            NavigationPage.Social => Social,
            NavigationPage.Settings => Settings,
            NavigationPage.Profile => Profile,
            _ => null
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _navigation.PageChanged -= OnPageChanged;
    }
}
