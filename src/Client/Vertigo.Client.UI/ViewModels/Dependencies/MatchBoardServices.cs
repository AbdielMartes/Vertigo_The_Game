using Vertigo.Client.Core.Navigation;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MatchBoardServices(
    ITextLocalizer Text, ISessionGuard Guard, IClientErrorText Errors, INavigator Navigation);
