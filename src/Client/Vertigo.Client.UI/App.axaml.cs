using Vertigo.Client.UI.Utilities.Settings.Interfaces;
using Vertigo.Client.UI.Utilities.Settings.Services;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Client.Networking;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.ViewModels;
using Vertigo.Client.UI.ViewModels.Dependencies;
using Vertigo.Client.UI.Views;
using Vertigo.Client.UI.ViewModels.Social;
using Vertigo.Client.UI.ViewModels.Utilities;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Services;

namespace Vertigo.Client.UI;

public partial class App : Application
{
    private ServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ServiceCollection registrations = new();
            registrations.AddVertigoClientNetworking();
            registrations.AddSingleton<ITextLocalizer, TextLocalizer>();
            registrations.AddSingleton<IClientErrorText, ClientErrorText>();
            registrations.AddSingleton<ISessionGuard, SessionGuard>();
            registrations.AddTransient<UiServices>();
            registrations.AddTransient<UiNavigationContext>();
            registrations.AddTransient<AccessContext>();
            registrations.AddTransient<LobbyContext>();
            registrations.AddTransient<HomeContext>();
            registrations.AddSingleton<IUserSettingsStore, JsonUserSettingsStore>();
            registrations.AddSingleton<SettingsViewModel>();
            registrations.AddSingleton<ProfileViewModel>();
            registrations.AddSingleton<RankingViewModel>();
            registrations.AddTransient<MainScreensContext>();
            registrations.AddTransient<MainContext>();
            registrations.AddSingleton<AccessViewModel>();
            registrations.AddSingleton<PlayViewModel>();
            registrations.AddSingleton<HomeViewModel>();
            registrations.AddTransient<MatchBoardContext>();
            registrations.AddSingleton<MatchViewModel>(provider =>
                new MatchViewModel(provider.GetRequiredService<MatchBoardContext>(),
                    provider.GetRequiredService<IChatClient>()));
            registrations.AddSingleton<LobbyViewModel>();
            registrations.AddSingleton<SocialOperation>();
            registrations.AddSingleton<FriendsViewModel>();
            registrations.AddSingleton<BlockedPlayersViewModel>();
            registrations.AddTransient<LobbyInvitationContext>();
            registrations.AddSingleton<LobbyInvitationsViewModel>();
            registrations.AddTransient<SocialNavigationContext>();
            registrations.AddTransient<SocialViewModelContext>();
            registrations.AddTransient<MainSocialContext>();
            registrations.AddSingleton<SocialViewModel>();
            registrations.AddSingleton<MainViewModel>();
            _services = registrations.BuildServiceProvider();
            UiLanguage.Apply(_services.GetRequiredService<IUserSettingsStore>().Load().Language);
            desktop.MainWindow = new MainWindow
            {
                DataContext = _services.GetRequiredService<MainViewModel>()
            };
            desktop.Exit += OnDesktopExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs eventArgs)
    {
        _services?.Dispose();
        _services = null;
    }
}
