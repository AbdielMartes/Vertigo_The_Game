using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record LobbyContext(PlayerSession PlayerSession, UiNavigationContext Ui,
    MatchViewModel Match);
