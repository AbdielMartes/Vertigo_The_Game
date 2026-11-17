using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MatchBoardContext(
    ILobbyClient Lobbies,
    PlayerSession Session,
    MatchBoardServices Services)
{
    public ITextLocalizer Text => Services.Text;
    public ISessionGuard Guard => Services.Guard;
    public IClientErrorText Errors => Services.Errors;
}
