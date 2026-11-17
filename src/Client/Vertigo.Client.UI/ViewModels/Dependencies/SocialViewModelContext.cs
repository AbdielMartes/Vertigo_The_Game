using Vertigo.Client.Core.Navigation;
using Vertigo.Client.Core.Sessions;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;
using Vertigo.Client.UI.Utilities.Sessions.Interfaces;
using Vertigo.Client.UI.ViewModels.Social;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record SocialViewModelContext(
    LobbyInvitationsViewModel Invitations,
    IFriendshipNotificationClient Notifications,
    SocialNavigationContext NavigationContext);
