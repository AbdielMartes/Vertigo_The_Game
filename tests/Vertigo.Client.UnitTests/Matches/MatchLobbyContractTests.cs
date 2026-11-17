using System.Reflection;
using Vertigo.Client.Networking.Interfaces;
using Vertigo.Contracts.Interfaces;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchLobbyContractTests
{
    [Fact]
    public void GetCurrentLobby_ServiceAndClient_DeclareMatchingResult()
    {
        MethodInfo? serviceMethod = typeof(ILobbyService).GetMethod(nameof(ILobbyService.GetCurrentLobby));
        MethodInfo? clientMethod = typeof(ILobbyClient).GetMethod(nameof(ILobbyClient.GetCurrentLobby));

        Assert.NotNull(serviceMethod);
        Assert.NotNull(clientMethod);
        Assert.Equal(serviceMethod.ReturnType, clientMethod.ReturnType);
    }

    [Fact]
    public void GetMatchBoard_Contract_HasOneSessionTokenAndOneLobbyCode()
    {
        MethodInfo? method = typeof(ILobbyService).GetMethod(nameof(ILobbyService.GetMatchBoard));

        Assert.NotNull(method);
        Assert.Equal(2, method.GetParameters().Length);
    }
}
