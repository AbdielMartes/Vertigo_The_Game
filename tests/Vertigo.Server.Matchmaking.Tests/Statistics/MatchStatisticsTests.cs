using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Internal;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Repositories.Statistics;
using Vertigo.Server.Matchmaking.Tests.Lobbies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Statistics;

namespace Vertigo.Server.Matchmaking.Tests.Statistics;

public sealed class MatchStatisticsTests
{
    [Fact]
    public async Task Record_SharedVictoryCountsForBothAndRankingBreaksTiesByUsername()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using IServiceScope scope = server.Services.CreateScope();
        var sessions = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        var recorder = scope.ServiceProvider.GetRequiredService<CompletedMatchRecorder>();
        var repository = scope.ServiceProvider.GetRequiredService<MatchStatisticsRepository>();
        DateTime expires = DateTime.UtcNow.AddMinutes(5);
        var first = sessions.Require(server.SeedRegisteredSession(expires, false));
        var second = sessions.Require(server.SeedRegisteredSession(expires, false));
        var third = sessions.Require(server.SeedRegisteredSession(expires, false));
        CompletedMatch tie = new(Guid.NewGuid(), DateTime.UtcNow, 3, 25, null,
        [
            new CompletedMatchPlayer(first.PlayerId, true, 18, 12),
            new CompletedMatchPlayer(second.PlayerId, true, 18, 12),
            new CompletedMatchPlayer(third.PlayerId, false, 20, 13)
        ]);

        await recorder.RecordAsync(tie, CancellationToken.None);

        var ranked = repository.GetRanking(3);
        Assert.Equal(new[] { first.Username, second.Username }.Order(StringComparer.Ordinal),
            ranked.Take(2).Select(player => player.Username));
        Assert.All(ranked.Take(2), player => Assert.Equal(1, player.MatchesWon));
        Assert.Equal(third.Username, ranked[2].Username);
        Assert.Equal(0, ranked[2].MatchesWon);
        Assert.Equal(2, repository.GetRanking(2).Count);
    }

    [Fact]
    public async Task Record_RegisteredWinnerAgainstGuestAndGuestWinner_OnlyCountsRegisteredStatistics()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using IServiceScope scope = server.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        var recorder = scope.ServiceProvider.GetRequiredService<CompletedMatchRecorder>();
        var sessions = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        Guid player = sessions.Require(server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false)).PlayerId;
        Guid guest = server.Guests.Create().PlayerId;
        CompletedMatch won = new(Guid.NewGuid(), DateTime.UtcNow, 3, 25, player,
            [new CompletedMatchPlayer(player, true, 18, 12), new CompletedMatchPlayer(guest, false, 4, 4)]);
        CompletedMatch lost = new(Guid.NewGuid(), DateTime.UtcNow, 3, 18, guest,
            [new CompletedMatchPlayer(player, false, 8, 7), new CompletedMatchPlayer(guest, true, 10, 8)]);

        await recorder.RecordAsync(won, CancellationToken.None);
        await recorder.RecordAsync(lost, CancellationToken.None);
        await recorder.RecordAsync(won, CancellationToken.None);

        var record = await database.PlayerStats.SingleAsync();
        Assert.Equal(player, record.PlayerId);
        Assert.Equal((2, 1, 26, 12), (record.MatchesPlayed, record.MatchesWon, record.TotalFloorsBuilt,
            record.HighestSkyscraper));
        Assert.Equal(player, (await database.MatchHistory.SingleAsync(match => match.MatchId == won.MatchId)).WinnerId);
        Assert.Null((await database.MatchHistory.SingleAsync(match => match.MatchId == lost.MatchId)).WinnerId);
        Assert.Equal(2, await database.MatchHistory.CountAsync());
    }
}
