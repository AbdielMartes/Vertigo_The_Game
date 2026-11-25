using System.ServiceModel;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Server.Matchmaking.Tests.Lobbies;

namespace Vertigo.Server.Matchmaking.Tests.Registration;

public sealed class ProfileUpdateNetTcpTests
{
    [Fact]
    public async Task UpdateProfile_RegisteredChangesReachSessionLobbyAndMatch()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
        string guest = server.Guests.Create().Token;
        using ChannelFactory<IAccountService> accounts = OpenFactory<IAccountService>(server, "/AccountService");
        using ChannelFactory<IAuthService> auth = OpenFactory<IAuthService>(server, "/AuthService");
        using ChannelFactory<IStatisticsService> stats = OpenFactory<IStatisticsService>(server, "/StatisticsService");
        IAccountService account = accounts.CreateChannel();
        IAuthService authentication = auth.CreateChannel();
        IStatisticsService statistics = stats.CreateChannel();

        try
        {
            ProfileUpdate edit = new() { Username = " TorreNueva ", AvatarId = 6, CountryCode = " mx " };
            Assert.True(account.UpdateProfile(edit, token));

            UserSession session = authentication.ResumeSession(token);
            Assert.Equal(("TorreNueva", 6), (session.Username, session.AvatarId));
            Assert.Equal("MX", statistics.GetMyRecord(token).CountryCode);
            VerifyLobbyAndMatch(server.Channel, token, guest, session.PlayerId);
        }
        finally
        {
            ((IClientChannel)account).Abort();
            ((IClientChannel)authentication).Abort();
            ((IClientChannel)statistics).Abort();
            accounts.Abort();
            auth.Abort();
            stats.Abort();
        }
    }

    [Fact]
    public async Task UpdateProfile_RejectsInvalidFieldsDuplicateNameAndGuest_AndCanClearCountry()
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        string token = server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
        string other = server.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
        string guest = server.Guests.Create().Token;
        using ChannelFactory<IAccountService> accounts = OpenFactory<IAccountService>(server, "/AccountService");
        using ChannelFactory<IStatisticsService> stats = OpenFactory<IStatisticsService>(server, "/StatisticsService");
        IAccountService account = accounts.CreateChannel();
        IStatisticsService statistics = stats.CreateChannel();

        try
        {
            Assert.True(account.UpdateProfile(new ProfileUpdate { Username = "OtraTorre" }, other));
            ProfileUpdate valid = new() { Username = "MiTorre", AvatarId = 2, CountryCode = "CL" };
            Assert.True(account.UpdateProfile(valid, token));
            VerifyFault(account, token, null!, ServiceErrorCodes.InvalidData);
            VerifyFault(account, token, new ProfileUpdate { Username = "xx" }, ServiceErrorCodes.InvalidData);
            VerifyFault(account, token, new ProfileUpdate { Username = "MiTorre", AvatarId = 999 },
                ServiceErrorCodes.InvalidData);
            VerifyFault(account, token, new ProfileUpdate { Username = "MiTorre", CountryCode = "ZZ" },
                ServiceErrorCodes.InvalidData);
            VerifyFault(account, token, new ProfileUpdate { Username = "OtraTorre", CountryCode = "AR" },
                ServiceErrorCodes.UsernameAlreadyRegistered);
            VerifyFault(account, guest, valid, ServiceErrorCodes.Unauthorized);
            PlayerMatchRecord before = statistics.GetMyRecord(token);
            Assert.Equal(("CL", 2), (before.CountryCode, before.AvatarId));

            valid.CountryCode = " za ";
            Assert.True(account.UpdateProfile(valid, token));
            Assert.Equal("ZA", statistics.GetMyRecord(token).CountryCode);

            valid.CountryCode = null;
            Assert.True(account.UpdateProfile(valid, token));
            Assert.Null(statistics.GetMyRecord(token).CountryCode);
        }
        finally
        {
            ((IClientChannel)account).Abort();
            ((IClientChannel)statistics).Abort();
            accounts.Abort();
            stats.Abort();
        }
    }

    private static ChannelFactory<T> OpenFactory<T>(LobbyHarness server, string path)
    {
        UriBuilder address = new(server.Address.Uri) { Path = path };
        return new ChannelFactory<T>(server.Binding, new EndpointAddress(address.Uri));
    }

    private static void VerifyFault(IAccountService account, string token, ProfileUpdate edit, string errorCode)
    {
        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => account.UpdateProfile(edit, token));
        Assert.Equal(errorCode, fault.Detail.ErrorCode);
    }

    private static void VerifyLobbyAndMatch(ILobbyService lobby, string token, string guest, Guid playerId)
    {
        string code = lobby.CreateLobby(true, 2, token);
        LobbyAdmission admission = lobby.JoinLobby(code, guest);
        LobbyMember member = Assert.Single(admission.ExistingMembers);
        Assert.Equal(("TorreNueva", 6), (member.Username, member.AvatarId));
        lobby.SetReady(code, true, token);
        lobby.SetReady(code, true, guest);
        lobby.StartMatch(code, token);

        MatchBoardSnapshot board = lobby.GetMatchBoard(code, guest);
        MatchBoardPlayer player = Assert.Single(board.Players, candidate => candidate.PlayerId == playerId);
        Assert.Equal(("TorreNueva", 6), (player.Username, player.AvatarId));
        Assert.Null(Assert.Single(board.Players, candidate => candidate.PlayerId != playerId).AvatarId);
    }
}
