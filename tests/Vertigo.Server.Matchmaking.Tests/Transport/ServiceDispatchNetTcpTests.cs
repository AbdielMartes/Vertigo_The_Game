using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.Host.Logging;
using Vertigo.Server.Matchmaking.Tests.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Transport;

public sealed class ServiceDispatchNetTcpTests
{
    [Fact]
    public async Task EmptyTransportAndSoapBody_DoNotExposeDetailsOrDisableEndpoint()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using (TcpClient transport = new())
        {
            await transport.ConnectAsync(server.Address.Uri.Host, server.Address.Uri.Port);
        }

        EndpointAddress endpoint = GetStatisticsAddress(server);
        using ChannelFactory<IEmptyStatistics> factory = new(server.Binding, endpoint);
        IEmptyStatistics channel = factory.CreateChannel();
        using Message request = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, RankingAction);
        try
        {
            using Message reply = channel.GetRanking(request);
            Assert.True(reply.IsFault);
            MessageFault fault = MessageFault.CreateFault(reply, 65536);
            ServiceFault detail = fault.GetDetail<ServiceFault>();
            Assert.Equal(ServiceErrorCodes.InvalidData, detail.ErrorCode);
            Assert.Equal(ServiceMessageKeys.InvalidData, detail.MessageKey);
        }
        finally
        {
            ((IClientChannel)channel).Abort();
            factory.Abort();
        }

        Assert.Contains("GetRanking", Assert.Single(server.Log.Messages(2101)), StringComparison.Ordinal);
        Assert.Empty(server.Log.Messages(2003));
        Assert.Empty(server.Log.Messages(2102));
        VerifyValidRanking(server, endpoint);
    }

    [Fact]
    public async Task Ranking_DatabaseFailureUsesOnePublicFaultAndLogsTheTechnicalCause()
    {
        DirectoryInfo logs = Directory.CreateTempSubdirectory();
        try
        {
            using IHost environment = new HostBuilder().Build();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(
                new Dictionary<string, string?> { [LoggingSettings.DirectoryKey] = logs.FullName }).Build();
            using (var logger = ServerLoggingConfiguration.CreateLogger(configuration,
                environment.Services.GetRequiredService<IHostEnvironment>()))
            {
                await VerifyDatabaseFailureAsync(logger);
                logger.ForContext("SourceContext", "Microsoft.EntityFrameworkCore.Database.Command")
                    .Error("Unscoped database diagnostic.");
            }

            string[] entries = File.ReadAllLines(Directory.GetFiles(Path.Combine(logs.FullName, "Technical")).Single());
            string entry = Assert.Single(entries,
                line => line.Contains("UnexpectedServiceFailure", StringComparison.Ordinal));
            Assert.Contains("GetRanking", entry, StringComparison.Ordinal);
            Assert.Contains("SqliteException", entry, StringComparison.Ordinal);
            Assert.DoesNotContain(entries, line => line.Contains("no such table", StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(entries, line => line.Contains("Failed executing DbCommand", StringComparison.Ordinal));
            Assert.DoesNotContain(entries, line => line.Contains("UnavailableStats", StringComparison.Ordinal));
            Assert.Single(entries, line => line.Contains("Unscoped database diagnostic", StringComparison.Ordinal));
        }
        finally
        {
            logs.Delete(recursive: true);
        }
    }

    private static async Task VerifyDatabaseFailureAsync(Serilog.ILogger logger)
    {
        await using LobbyHarness server = await LobbyHarness.StartWithTechnicalLogAsync(logger);
        EndpointAddress endpoint = GetStatisticsAddress(server);
        using ChannelFactory<IStatisticsService> factory = new(server.Binding, endpoint);
        IStatisticsService channel = factory.CreateChannel();
        string token = server.Guests.Create().Token;
        using IServiceScope scope = server.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        database.Database.ExecuteSqlRaw("ALTER TABLE PlayerStats RENAME TO UnavailableStats");
        try
        {
            var fault = Assert.Throws<FaultException<ServiceFault>>(() => channel.GetRanking(token));
            Assert.Equal(ServiceErrorCodes.Unexpected, fault.Detail.ErrorCode);
            Assert.Equal(ServiceMessageKeys.Unexpected, fault.Detail.MessageKey);
            Assert.DoesNotContain("no such table", fault.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("UnavailableStats", fault.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(token, fault.Message, StringComparison.Ordinal);
            database.Database.ExecuteSqlRaw("ALTER TABLE UnavailableStats RENAME TO PlayerStats");
            Assert.Empty(channel.GetRanking(token));
        }
        finally
        {
            ((IClientChannel)channel).Abort();
            factory.Abort();
        }
    }

    private static void VerifyValidRanking(LobbyHarness server, EndpointAddress endpoint)
    {
        using ChannelFactory<IStatisticsService> factory = new(server.Binding, endpoint);
        IStatisticsService channel = factory.CreateChannel();
        try
        {
            var fault = Assert.Throws<FaultException<ServiceFault>>(() => channel.GetRanking(null!));
            Assert.Equal(ServiceErrorCodes.Unauthorized, fault.Detail.ErrorCode);
            Assert.Single(server.Log.Messages(2003));
            Assert.Empty(channel.GetRanking(server.Guests.Create().Token));
        }
        finally
        {
            ((IClientChannel)channel).Abort();
            factory.Abort();
        }
    }

    private static EndpointAddress GetStatisticsAddress(LobbyHarness server)
    {
        return new EndpointAddress(new UriBuilder(server.Address.Uri) { Path = "/StatisticsService" }.Uri);
    }

    private const string RankingAction = "http://tempuri.org/IStatisticsService/GetRanking";

    [ServiceContract]
    public interface IEmptyStatistics
    {
        [OperationContract(Action = RankingAction, ReplyAction = "*")]
        Message GetRanking(Message request);
    }
}
