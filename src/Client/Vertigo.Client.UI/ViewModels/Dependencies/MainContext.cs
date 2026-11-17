namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainContext(HomeViewModel Home, MainSocialContext SocialContext,
    MainScreensContext Screens)
{
    public LobbyViewModel Lobby => Screens.Lobby;
    public SettingsViewModel Settings => Screens.Settings;
    public ProfileViewModel Profile => Screens.Profile;
}
