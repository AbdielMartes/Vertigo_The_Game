using Vertigo.Client.Core.Navigation;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiNavigationContext(INavigator Navigation, UiServices Services);
