using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record AccessContext(ISessionCoordinator Sessions, UiNavigationContext Ui);
