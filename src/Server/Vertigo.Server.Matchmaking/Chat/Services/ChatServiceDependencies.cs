using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.Matchmaking.Chat.Services;

public sealed record ChatServiceDependencies(
    ISessionIdentityResolver Sessions,
    TimeProvider TimeProvider,
    ChatLanguageFilter LanguageFilter);
