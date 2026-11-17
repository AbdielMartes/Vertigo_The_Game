using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record UiServices(IClientErrorText Errors, ITextLocalizer Text,
    ISessionGuard SessionGuard);
