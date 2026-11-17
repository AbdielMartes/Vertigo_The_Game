using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record HomeContext(ISessionCoordinator Sessions, PlayerSession PlayerSession,
    UiNavigationContext Ui);
