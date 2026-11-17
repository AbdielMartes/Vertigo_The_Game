using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record LobbyInvitationContext(LobbyViewModel Lobby, ITextLocalizer Text);
