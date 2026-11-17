using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record SocialNavigationContext(PlayerSession Session, INavigator Navigation,
    UiServices Services);
