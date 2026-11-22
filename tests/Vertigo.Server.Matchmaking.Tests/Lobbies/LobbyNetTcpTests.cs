using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Endpoints;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Security.Implementations;
using Vertigo.Server.DataAccess.Contexts;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class LobbyNetTcpTests
{
    [Theory]
    [InlineData(null, "MissingToken")]
    [InlineData("", "MissingToken")]
    [InlineData(" ", "MissingToken")]
    [InlineData("bad-token", "MalformedToken")]
    [InlineData("expired", "UnknownOrInactiveToken")]
    [InlineData("revoked", "UnknownOrInactiveToken")]
    [InlineData("unknown", "UnknownOrInactiveToken")]
    public async Task CreateLobby_RejectedSession_LeavesNoLobbyAndChannelUsable(string? scenario, string reason)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string active = server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
        string? rejectedToken = scenario switch
        {
            "expired" => server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(-5), false),
            "revoked" => server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), true),
            "unknown" => new TokenGenerator().GenerateSessionToken(),
            _ => scenario
        };

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.CreateLobby(true, 2, rejectedToken!));

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
        Assert.Equal(ServiceMessageKeys.Unauthorized, fault.Detail.MessageKey);
        string entry = Assert.Single(server.Log.Messages(2003));
        Assert.Contains(nameof(ILobbyService.CreateLobby), entry, StringComparison.Ordinal);
        Assert.Contains(reason, entry, StringComparison.Ordinal);
        if (rejectedToken is { Length: 64 })
        {
            Assert.DoesNotContain(rejectedToken, entry, StringComparison.Ordinal);
        }
        AssertSuccessfulCreation(server.Channel, active, 2);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(7, 6)]
    public async Task CreateLobby_OutsideCapacity_RejectsAndAcceptsNearestLimit(int invalid, int boundary)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.CreateLobby(true, invalid, token));

        Assert.Equal(ServiceErrorCodes.InvalidLobbyCapacity, fault.Detail.ErrorCode);
        Assert.Empty(server.Log.Messages(2003));
        AssertSuccessfulCreation(server.Channel, token, boundary);
    }

    [Fact]
    public async Task JoinLobby_NullRequiredCodeWithValidSession_RejectsAndKeepsChannelUsable()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.JoinLobby(null!, token));

        Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
        Assert.Empty(server.Log.Messages(2003));
        Assert.Empty(server.Log.Messages(2101));
        AssertSuccessfulCreation(server.Channel, token, 2);
    }

    [Fact]
    public async Task CreateLobby_InvalidCapacityType_IsLoggedByCoreWcfBeforeInvocation()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;
        using DuplexChannelFactory<IInvalidCapacityLobby> factory = new(
            new InstanceContext(new LobbyCallbacks()), server.Binding, server.Address);
        IInvalidCapacityLobby channel = factory.CreateChannel();

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => channel.CreateLobby(true, "invalid-capacity", token));

        Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
        string entry = Assert.Single(server.Log.Messages(2101));
        Assert.Contains(nameof(ILobbyService.CreateLobby), entry, StringComparison.Ordinal);
        Assert.DoesNotContain("invalid-capacity", entry, StringComparison.Ordinal);
        Assert.DoesNotContain(token, entry, StringComparison.Ordinal);
        AssertSuccessfulCreation(server.Channel, token, 2);
        ((IClientChannel)channel).Close();
    }

    [Fact]
    public async Task CreateLobby_DatabaseReadFails_LogsGenericFaultAndRecovers()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
        using IServiceScope scope = server.Services.CreateScope();
        VertigoDatabaseContext database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        database.Database.ExecuteSqlRaw("ALTER TABLE SessionTokens RENAME TO UnavailableSessions");

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.CreateLobby(true, 2, token));

        Assert.Equal(ServiceErrorCodes.Unexpected, fault.Detail.ErrorCode);
        Assert.Equal(ServiceMessageKeys.Unexpected, fault.Detail.MessageKey);
        string entry = Assert.Single(server.Log.Messages(2102));
        Assert.Contains(nameof(ILobbyService.CreateLobby), entry, StringComparison.Ordinal);
        Assert.Contains(nameof(SqliteException), entry, StringComparison.Ordinal);
        Assert.DoesNotContain("UnavailableSessions", fault.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(token, entry, StringComparison.Ordinal);
        database.Database.ExecuteSqlRaw("ALTER TABLE UnavailableSessions RENAME TO SessionTokens");
        AssertSuccessfulCreation(server.Channel, token, 2);
    }

    [Fact]
    public async Task CreateLobby_PlayerAlreadyInLobby_PreservesExistingRoom()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;
        string original = server.Channel.CreateLobby(true, 2, token);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.CreateLobby(true, 6, token));

        Assert.Equal(ServiceErrorCodes.PlayerAlreadyInLobby, fault.Detail.ErrorCode);
        LobbySummary lobby = Assert.Single(server.Channel.GetPublicLobbies());
        Assert.Equal(original, lobby.LobbyCode);
        Assert.Equal(2, lobby.MaxPlayers);
        Assert.Empty(server.Log.Messages(2102));
    }

    [Fact]
    public async Task GetPreparation_Outsider_LogsSecurityRejectionAndPreservesLobby()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string host = server.Guests.Create().Token;
        string outsider = server.Guests.Create().Token;
        string code = server.Channel.CreateLobby(true, 2, host);

        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.GetPreparation(code, outsider));

        Assert.Equal(ServiceErrorCodes.LobbyNotMember, fault.Detail.ErrorCode);
        string entry = Assert.Single(server.Log.Messages(2103));
        Assert.Contains(nameof(ILobbyService.GetPreparation), entry, StringComparison.Ordinal);
        Assert.Contains(ServiceErrorCodes.LobbyNotMember, entry, StringComparison.Ordinal);
        Assert.DoesNotContain(outsider, entry, StringComparison.Ordinal);
        Assert.Single(server.Channel.GetPublicLobbies());
        Assert.Equal(code, server.Channel.GetPreparation(code, host).LobbyCode);
    }

    [Fact]
    public async Task CreateLobby_OnlySoapHeader_UsesSessionAndLeavesOtherCallsUnchanged()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;

        string code;
        using (OperationContextScope operation = AddHeader(server.Channel, token))
        {
            code = server.Channel.CreateLobby(true, 2, null);
        }

        LobbySummary lobby = Assert.Single(server.Channel.GetPublicLobbies());
        Assert.Equal(code, lobby.LobbyCode);
        Assert.Equal(code, server.Channel.GetCurrentLobby(token)?.LobbyCode);
        Assert.True(server.Channel.LeaveLobby(code, token));
        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => server.Channel.CreateLobby(true, 2, null));
        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
        AssertSuccessfulCreation(server.Channel, token, 2);
    }

    [Theory]
    [InlineData(false, "MultipleCredentialSources")]
    [InlineData(true, "DuplicateSessionHeader")]
    public async Task CreateLobby_AmbiguousSoapCredential_IsRejectedWithoutCreatingRoom(bool duplicateHeader,
        string reason)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;
        string? bodyToken = duplicateHeader ? null : token;
        FaultException<ServiceFault> fault;

        using (OperationContextScope operation = AddHeader(server.Channel, token))
        {
            if (duplicateHeader)
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(
                    MessageHeader.CreateHeader(SessionHeader.Name, SessionHeader.Namespace, token));
            }

            fault = Assert.Throws<FaultException<ServiceFault>>(
                () => server.Channel.CreateLobby(true, 2, bodyToken));
        }

        Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
        string entry = Assert.Single(server.Log.Messages(2104));
        Assert.Contains(reason, entry, StringComparison.Ordinal);
        Assert.DoesNotContain(token, entry, StringComparison.Ordinal);
        AssertSuccessfulCreation(server.Channel, token, 2);
    }

    [Fact]
    public async Task CreateLobby_StructuredSoapHeader_IsRejectedWithoutCreatingRoom()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.Guests.Create().Token;
        FaultException<ServiceFault> fault;

        using (OperationContextScope operation = new((IClientChannel)server.Channel))
        {
            OperationContext.Current.OutgoingMessageHeaders.Add(
                MessageHeader.CreateHeader(SessionHeader.Name, SessionHeader.Namespace, new[] { token }));
            fault = Assert.Throws<FaultException<ServiceFault>>(
                () => server.Channel.CreateLobby(true, 2, null));
        }

        Assert.Equal(ServiceErrorCodes.InvalidData, fault.Detail.ErrorCode);
        string entry = Assert.Single(server.Log.Messages(2104));
        Assert.Contains("InvalidSessionHeader", entry, StringComparison.Ordinal);
        Assert.DoesNotContain(token, entry, StringComparison.Ordinal);
        AssertSuccessfulCreation(server.Channel, token, 2);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CreateLobby_InactiveTokenInSoapHeader_IsRejected(bool revoked)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        DateTime expiresAt = revoked ? DateTime.UtcNow.AddMinutes(5) : DateTime.UtcNow.AddMinutes(-5);
        string token = server.SeedRegisteredSession(expiresAt, revoked);
        FaultException<ServiceFault> fault;

        using (OperationContextScope operation = AddHeader(server.Channel, token))
        {
            fault = Assert.Throws<FaultException<ServiceFault>>(
                () => server.Channel.CreateLobby(true, 2, null));
        }

        Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
        Assert.Contains("UnknownOrInactiveToken", Assert.Single(server.Log.Messages(2003)), StringComparison.Ordinal);
        AssertSuccessfulCreation(server.Channel, server.Guests.Create().Token, 2);
    }

    private static OperationContextScope AddHeader(ILobbyService channel, string token)
    {
        OperationContextScope scope = new((IClientChannel)channel);
        OperationContext.Current.OutgoingMessageHeaders.Add(
            MessageHeader.CreateHeader(SessionHeader.Name, SessionHeader.Namespace, token));
        return scope;
    }

    private static void AssertSuccessfulCreation(ILobbyService channel, string token, int capacity)
    {
        Assert.Empty(channel.GetPublicLobbies());
        Assert.Null(channel.GetCurrentLobby(token));

        string code = channel.CreateLobby(true, capacity, token);

        Assert.Equal(6, code.Length);
        LobbySummary lobby = Assert.Single(channel.GetPublicLobbies());
        Assert.Equal(code, lobby.LobbyCode);
        Assert.Equal(capacity, lobby.MaxPlayers);
    }

    [ServiceContract(Name = nameof(ILobbyService), CallbackContract = typeof(ILobbyCallback),
        SessionMode = SessionMode.Required)]
    public interface IInvalidCapacityLobby
    {
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        string CreateLobby(bool isPublic, string maxPlayers, string sessionToken);
    }

    private sealed class LobbyCallbacks : ILobbyCallback
    {
        public void Notify(LobbyNotification notification)
        {
        }
    }
}
