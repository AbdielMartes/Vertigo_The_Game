using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MatchBoardContext(
    ILobbyClient Lobbies,
    PlayerSession Session,
    UiNavigationContext Ui)
{
    public ITextLocalizer Text => Ui.Services.Text;
    public ISessionGuard Guard => Ui.Services.SessionGuard;
    public IClientErrorText Errors => Ui.Services.Errors;
}
