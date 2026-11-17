using Vertigo.Client.Core.Sessions;

namespace Vertigo.Client.UI.ViewModels.Dependencies;

public sealed record MainSocialContext(PlayerSession PlayerSession, SocialViewModel Social);
