using System.ServiceModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.DataAccess.Entities.Enums;
using Vertigo.Server.DataAccess.Entities.Identity;
using Vertigo.Server.Matchmaking.Tests.Lobbies;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;

namespace Vertigo.Server.Matchmaking.Tests.Statistics;

public sealed class RankingProfileTests
{
    [Fact]
    public async Task Rankings_UseAcceptedFriendsAndSavedCountry_AndExposePublicProfile()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        DateTime expiry = DateTime.UtcNow.AddMinutes(5);
        string viewer = server.SeedRegisteredSession(expiry, false);
        string friend = server.SeedRegisteredSession(expiry, false);
        string stranger = server.SeedRegisteredSession(expiry, false);
        string blocker = server.SeedRegisteredSession(expiry, false);
        using IServiceScope scope = server.Services.CreateScope();
        VertigoDatabaseContext database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        ISessionIdentityResolver identities = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>();
        Guid viewerId = identities.RequireRegistered(viewer).PlayerId;
        Guid friendId = identities.RequireRegistered(friend).PlayerId;
        Guid strangerId = identities.RequireRegistered(stranger).PlayerId;
        Guid blockerId = identities.RequireRegistered(blocker).PlayerId;
        SeedRanking(database, viewerId, friendId, strangerId, blockerId);
        UriBuilder endpoint = new(server.Address.Uri) { Path = "/StatisticsService" };
        using ChannelFactory<IStatisticsService> factory = new(server.Binding, new EndpointAddress(endpoint.Uri));
        IStatisticsService statistics = factory.CreateChannel();

        try
        {
            RankingEntry[] friends = statistics.GetFriendsRanking(viewer).ToArray();
            Assert.Equal(new[] { friendId, viewerId }, friends.Select(entry => entry.PlayerId));
            Assert.Equal(6, friends[0].AvatarId);
            Assert.DoesNotContain(statistics.GetRanking(viewer), entry => entry.PlayerId == blockerId);
            string guest = server.Guests.Create().Token;
            Assert.Equal(ServiceErrorCodes.Unauthorized, Assert.Throws<FaultException<ServiceFault>>(
                () => statistics.GetFriendsRanking(guest)).Detail.ErrorCode);
            Assert.Equal(ServiceErrorCodes.Unauthorized, Assert.Throws<FaultException<ServiceFault>>(
                () => statistics.GetCountryRanking(guest)).Detail.ErrorCode);
            RankingEntry country = Assert.Single(statistics.GetCountryRanking(viewer));
            Assert.Equal(strangerId, country.PlayerId);
            PublicPlayerProfile profile = statistics.GetPlayerProfile(friendId, viewer);
            Assert.Equal((friends[0].Username, "AR", 6, 5), (profile.Username, profile.Record.CountryCode,
                profile.Record.AvatarId, profile.Record.MatchesPlayed));
            Assert.Equal(ServiceErrorCodes.UserNotFound, Assert.Throws<FaultException<ServiceFault>>(
                () => statistics.GetPlayerProfile(blockerId, viewer)).Detail.ErrorCode);

            SetCountry(database, viewerId, "AR");
            Assert.Equal(friendId, Assert.Single(statistics.GetCountryRanking(viewer)).PlayerId);
            SetCountry(database, viewerId, null);
            Assert.Equal(ServiceMessageKeys.RankingCountryRequired, Assert.Throws<FaultException<ServiceFault>>(
                () => statistics.GetCountryRanking(viewer)).Detail.MessageKey);
        }
        finally
        {
            ((IClientChannel)statistics).Abort();
            factory.Abort();
        }
    }

    private static void SeedRanking(VertigoDatabaseContext database, Guid viewer, Guid friend, Guid stranger,
        Guid blocker)
    {
        SetCountry(database, viewer, "MX");
        SetCountry(database, friend, "AR");
        SetCountry(database, stranger, "MX");
        SetCountry(database, blocker, "MX");
        database.Players.Where(player => player.PlayerId == friend)
            .ExecuteUpdate(setters => setters.SetProperty(player => player.AvatarId, 6));
        database.Friendships.AddRange(
            new Friendship { FriendshipId = Guid.NewGuid(), PlayerRequesterId = friend, PlayerAddresseeId = viewer,
                Status = FriendshipStatus.Accepted, CreatedAt = DateTime.UtcNow },
            new Friendship { FriendshipId = Guid.NewGuid(), PlayerRequesterId = viewer, PlayerAddresseeId = stranger,
                Status = FriendshipStatus.Pending, CreatedAt = DateTime.UtcNow });
        database.BlockedPlayers.Add(new BlockedPlayer
        {
            BlockId = Guid.NewGuid(), PlayerBlockerId = blocker, PlayerBlockedId = viewer, CreatedAt = DateTime.UtcNow
        });
        database.PlayerStats.AddRange(
            new PlayerStats { PlayerId = friend, MatchesPlayed = 5, MatchesWon = 4 },
            new PlayerStats { PlayerId = stranger, MatchesPlayed = 2, MatchesWon = 1 },
            new PlayerStats { PlayerId = blocker, MatchesPlayed = 2, MatchesWon = 2 });
        database.SaveChanges();
    }

    private static void SetCountry(VertigoDatabaseContext database, Guid playerId, string? country)
    {
        database.Players.Where(player => player.PlayerId == playerId)
            .ExecuteUpdate(setters => setters.SetProperty(player => player.CountryCode, country));
    }
}
