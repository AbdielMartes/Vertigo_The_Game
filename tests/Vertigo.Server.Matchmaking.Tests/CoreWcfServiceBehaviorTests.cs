using System.Reflection;
using CoreWCF;
using Vertigo.Server.Matchmaking.Chat.Services;
using Vertigo.Server.Matchmaking.Invitations.Services;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Services;
using Vertigo.Server.PlayerManagement.Blocking.Services;
using Vertigo.Server.PlayerManagement.Friendships.Services;
using Vertigo.Server.PlayerManagement.Registration.Services;

namespace Vertigo.Server.Matchmaking.Tests;

public sealed class CoreWcfServiceBehaviorTests
{
    [Theory]
    [InlineData(typeof(AccountService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(AuthenticationService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(BlockedPlayerService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(ChatService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(FriendshipService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(LobbyInvitationService), InstanceContextMode.PerCall, ConcurrencyMode.Single)]
    [InlineData(typeof(LobbyService), InstanceContextMode.Single, ConcurrencyMode.Multiple)]
    [InlineData(typeof(FriendshipNotificationService), InstanceContextMode.Single, ConcurrencyMode.Multiple)]
    public void ServiceBehavior_HostedService_DeclaresExpectedModes(
        Type service, InstanceContextMode instanceMode, ConcurrencyMode concurrencyMode)
    {
        ServiceBehaviorAttribute? behavior = service.GetCustomAttribute<ServiceBehaviorAttribute>(inherit: false);

        Assert.NotNull(behavior);
        Assert.Equal(instanceMode, behavior.InstanceContextMode);
        Assert.Equal(concurrencyMode, behavior.ConcurrencyMode);
    }
}
