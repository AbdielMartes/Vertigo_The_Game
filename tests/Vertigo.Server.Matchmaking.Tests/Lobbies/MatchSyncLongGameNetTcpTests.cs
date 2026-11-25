using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.DataAccess.Contexts;
using Vertigo.Server.Game.Matches.Models;
using Vertigo.Server.Host.Matches;
using Vertigo.Server.PlayerManagement.Authentication.Models;
using Vertigo.Server.PlayerManagement.Authentication.Interfaces;
using Vertigo.Server.PlayerManagement.Statistics;
using Xunit.Abstractions;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class MatchSyncLongGameNetTcpTests
{
    private const int RoundCount = 3;
    private const int TurnLimit = 600;
    private readonly ITestOutputHelper _output;

    public MatchSyncLongGameNetTcpTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task GetMatchUpdate_EachRoomSizeCompletesThreeRounds_SyncsEveryPrivateHand(int playerCount)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync(clock: new FixedClock(DateTimeOffset.UnixEpoch));
        using PlayerChannels players = new(server, playerCount);
        MatchIdentity match = players.StartMatch();
        Dictionary<Guid, MatchSyncState> baselines = ReadInitialStates(players, match);
        MatchBoardSnapshot board = baselines[players.Host.Session.PlayerId].Board;
        Assert.Equal(playerCount, board.Players.Count);
        VerifyUpdates(players, match, baselines);
        bool measuredThirdRoundTurn = false;

        for (int turn = 0; turn < TurnLimit && board.EndReason == MatchEndReason.None; turn++)
        {
            PlayerChannel active = players.For(board.CurrentTurnPlayerId);
            int completedRounds = board.RoundSummaries.Count;
            board = PlayTurn(active, match.LobbyCode, board);
            if (board.RoundSummaries.Count > completedRounds)
            {
                VerifyUpdates(players, match, baselines);
                Assert.Equal(playerCount, board.RoundSummaries[^1].Players.Count);
            }
            else if (!measuredThirdRoundTurn && completedRounds == RoundCount - 1)
            {
                VerifyUpdates(players, match, baselines);
                measuredThirdRoundTurn = true;
            }
        }

        Assert.Equal(MatchEndReason.Completed, board.EndReason);
        Assert.True(measuredThirdRoundTurn);
        Assert.Equal(RoundCount, board.RoundSummaries.Count);
        Assert.Equal(playerCount, board.Players.Count);
        Assert.Equal(playerCount, baselines.Count);
        Assert.All(baselines.Values, state => Assert.Equal(board.Revision, state.Board.Revision));
    }

    private static Dictionary<Guid, MatchSyncState> ReadInitialStates(PlayerChannels players, MatchIdentity match)
    {
        Dictionary<Guid, MatchSyncState> states = new();
        foreach (PlayerChannel player in players.All)
        {
            Guid playerId = player.Session.PlayerId;
            MatchSyncUpdate update = player.Channel.GetMatchUpdate(Request(match, player.Session));
            states.Add(playerId, MatchSynchronization.Apply(update, null, playerId));
        }

        return states;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LeaveMatch_AbandonmentRequiresAClosedRoundForWinnerAndNeverGeneratesStatistics(bool roundClosed)
    {
        await using LobbyHarness server = await LobbyHarness.StartAsync();
        using PlayerChannels players = new(server, 2, CreateRegisteredSession(server));
        MatchIdentity match = players.StartMatch();
        PlayerChannel guest = players.All[1];
        string token = players.Host.Session.Token;
        MatchBoardSnapshot active = players.Host.Channel.GetMatchBoard(match.LobbyCode, token);
        for (int turn = 0; roundClosed && turn < TurnLimit && active.RoundSummaries.Count == 0; turn++)
        {
            active = PlayTurn(players.For(active.CurrentTurnPlayerId), match.LobbyCode, active);
        }

        Assert.Equal(roundClosed ? 1 : 0, active.RoundSummaries.Count);

        bool left = guest.Channel.LeaveMatch(new MatchLeaveRequest
        {
            LobbyCode = match.LobbyCode, MatchId = match.MatchId, SessionToken = guest.Session.Token
        });

        Assert.True(left);
        MatchBoardSnapshot board = players.Host.Channel.GetMatchBoard(match.LobbyCode, token);
        Assert.Equal(MatchEndReason.Abandonment, board.EndReason);
        Assert.Equal(roundClosed ? new[] { players.Host.Session.PlayerId } : Array.Empty<Guid>(), board.WinnerIds);
        Assert.Equal(roundClosed ? 1 : null, Assert.Single(board.Players).FinalPosition);
        Assert.Empty(server.Services.GetRequiredService<ICompletedMatchSource>().GetPendingResults(16));
    }

    [Fact]
    public async Task StatisticsWorker_RepeatedWriteFailureDoesNotBlockOtherMatches()
    {
        DirectoryInfo directory = Directory.CreateTempSubdirectory();
        try
        {
            using RecoveryDirectory journal = new(directory.FullName);
            await using LobbyHarness server = await LobbyHarness.StartAsync(recovery: journal);
            using PlayerChannels firstRoom = new(server, 2, CreateRegisteredSession(server));
            using PlayerChannels secondRoom = new(server, 2, CreateRegisteredSession(server));
            Guid first = FinishMatch(firstRoom, firstRoom.StartMatch()).MatchId;
            Guid second = FinishMatch(secondRoom, secondRoom.StartMatch()).MatchId;
            ICompletedMatchSource source = server.Services.GetRequiredService<ICompletedMatchSource>();
            CompletedMatch rejected = source.GetPendingResults(16).Single(result => result.MatchId == first);
            TimeProvider clock = new FixedClock(new DateTimeOffset(rejected.CompletedAt).AddHours(2));
            using MatchStatisticsWorker worker = new(source, server.Services.GetRequiredService<IServiceScopeFactory>(),
                server.Services.GetRequiredService<ILogger<MatchStatisticsWorker>>(), clock);
            using IServiceScope scope = server.Services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
            var connection = (SqliteConnection)database.Database.GetDbConnection();
            connection.CreateFunction("BlockedResult", (string id) => Guid.Parse(id) == first);
            await database.Database.ExecuteSqlRawAsync(
                "CREATE TRIGGER RejectOneResult BEFORE INSERT ON MatchHistory "
                + "WHEN BlockedResult(NEW.MatchId) BEGIN SELECT RAISE(ABORT, 'rejected result'); END;");

            await worker.ProcessPendingAsync(CancellationToken.None);

            Assert.Equal(new[] { second, first }, source.GetPendingResults(16).Select(result => result.MatchId));
            Assert.Empty(await database.MatchHistory.ToListAsync());
            string warning = Assert.Single(server.Log.Messages(2301));
            Assert.Contains("7200 seconds", warning, StringComparison.Ordinal);
            Assert.DoesNotContain("rejected result", warning, StringComparison.Ordinal);

            await worker.ProcessPendingAsync(CancellationToken.None);

            Assert.Equal(second, (await database.MatchHistory.SingleAsync()).MatchId);
            Assert.Equal(first, Assert.Single(source.GetPendingResults(16)).MatchId);

            await database.Database.ExecuteSqlRawAsync("DROP TRIGGER RejectOneResult");
            string blockedMarker = Path.Combine(directory.FullName, "matches-v1", first.ToString("N"),
                "statistics-recorded.json.pending");
            Directory.CreateDirectory(blockedMarker);
            await worker.ProcessPendingAsync(CancellationToken.None);

            Assert.Equal(2, await database.MatchHistory.CountAsync());
            Assert.Equal(first, Assert.Single(source.GetPendingResults(16)).MatchId);

            Directory.Delete(blockedMarker);
            await worker.ProcessPendingAsync(CancellationToken.None);

            Assert.Empty(source.GetPendingResults(16));
            Assert.Equal(2, await database.MatchHistory.CountAsync());
            var statistics = await database.PlayerStats.ToListAsync();
            Assert.Equal(2, statistics.Count);
            Assert.All(statistics, stats => Assert.Equal(1, stats.MatchesPlayed));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task FinishedMatch_ReplaysUnacknowledgedResultAndRecordsStatisticsOnlyOnce()
    {
        DirectoryInfo directory = Directory.CreateTempSubdirectory();
        await using SqliteConnection database = new("Data Source=:memory:");
        await database.OpenAsync();
        try
        {
            (CompletedMatch expected, string token) = await FinishAndRecordAsync(database, directory.FullName);
            using (RecoveryDirectory journal = new(directory.FullName))
            await using (LobbyHarness restarted = await LobbyHarness.StartAsync(database, journal))
            {
                ICompletedMatchSource source = restarted.Services.GetRequiredService<ICompletedMatchSource>();
                CompletedMatch replayed = Assert.Single(source.GetPendingResults(16));
                Assert.Equal(expected.MatchId, replayed.MatchId);
                Assert.Equal(expected.Players, replayed.Players);
                await RecordResultsAsync(restarted, replayed, failFirstWrite: false);
                await VerifyRankingAsync(restarted, replayed, token);
                source.AcknowledgeResult(replayed.MatchId);
                Assert.Empty(source.GetPendingResults(16));
            }

            using RecoveryDirectory acknowledged = new(directory.FullName);
            await using LobbyHarness finalHost = await LobbyHarness.StartAsync(database, acknowledged);
            Assert.Empty(finalHost.Services.GetRequiredService<ICompletedMatchSource>().GetPendingResults(16));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    private static async Task<(CompletedMatch, string)> FinishAndRecordAsync(SqliteConnection database, string directory)
    {
        using RecoveryDirectory journal = new(directory);
        await using LobbyHarness server = await LobbyHarness.StartAsync(database, journal);
        using PlayerChannels players = new(server, 2, CreateRegisteredSession(server));
        MatchIdentity match = players.StartMatch();
        AuthenticatedSession host = players.Host.Session;
        MatchBoardSnapshot board = FinishMatch(players, match);
        CompletedMatch result = Assert.Single(
            server.Services.GetRequiredService<ICompletedMatchSource>().GetPendingResults(16));
        CompletedMatchPlayer player = Assert.Single(result.Players);
        Assert.Equal(host.PlayerId, player.PlayerId);
        Assert.Equal(board.WinnerIds.Contains(host.PlayerId), player.Won);
        Assert.Equal(board.RoundSummaries.SelectMany(round => round.Players)
            .Where(score => score.PlayerId == host.PlayerId).Sum(score => score.FloorsBuilt), player.FloorsBuilt);
        await RecordResultsAsync(server, result, failFirstWrite: true);
        return (result, host.Token);
    }

    private static MatchBoardSnapshot FinishMatch(PlayerChannels players, MatchIdentity match)
    {
        MatchBoardSnapshot board = players.Host.Channel.GetMatchBoard(match.LobbyCode, players.Host.Session.Token);
        for (int turn = 0; turn < TurnLimit && board.EndReason == MatchEndReason.None; turn++)
        {
            board = PlayTurn(players.For(board.CurrentTurnPlayerId), match.LobbyCode, board);
        }

        Assert.Equal(MatchEndReason.Completed, board.EndReason);
        return board;
    }

    private static AuthenticatedSession CreateRegisteredSession(LobbyHarness server)
    {
        DateTime expiration = DateTime.UtcNow.AddMinutes(5);
        string token = server.SeedRegisteredSession(expiration, false);
        using IServiceScope scope = server.Services.CreateScope();
        SessionIdentity identity = scope.ServiceProvider.GetRequiredService<ISessionIdentityResolver>().Require(token);
        return new AuthenticatedSession
        {
            Token = token, PlayerId = identity.PlayerId, Username = identity.Username, ExpiresAt = expiration
        };
    }

    private static async Task RecordResultsAsync(LobbyHarness server, CompletedMatch result, bool failFirstWrite)
    {
        using IServiceScope scope = server.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
        var recorder = scope.ServiceProvider.GetRequiredService<CompletedMatchRecorder>();
        if (failFirstWrite)
        {
            await database.Database.ExecuteSqlRawAsync(
                "CREATE TRIGGER RejectStatistics BEFORE INSERT ON PlayerStats "
                + "BEGIN SELECT RAISE(ABORT, 'statistics unavailable'); END;");
            await Assert.ThrowsAsync<DbUpdateException>(() => recorder.RecordAsync(result, CancellationToken.None));
            database.ChangeTracker.Clear();
            Assert.Empty(await database.MatchHistory.ToListAsync());
            Assert.Empty(await database.PlayerStats.ToListAsync());
            await database.Database.ExecuteSqlRawAsync("DROP TRIGGER RejectStatistics");
        }

        await recorder.RecordAsync(result, CancellationToken.None);
        Assert.Equal(result.MatchId, (await database.MatchHistory.SingleAsync()).MatchId);
        Assert.Equal(1, (await database.PlayerStats.SingleAsync()).MatchesPlayed);
    }

    private static async Task VerifyRankingAsync(LobbyHarness server, CompletedMatch result, string token)
    {
        UriBuilder address = new(server.Address.Uri) { Path = "/StatisticsService" };
        using ChannelFactory<IStatisticsService> factory = new(server.Binding, new EndpointAddress(address.Uri));
        IStatisticsService channel = factory.CreateChannel();
        try
        {
            CompletedMatchPlayer expected = Assert.Single(result.Players);
            string guest = server.Guests.Create().Token;
            RankingEntry entry = Assert.Single(channel.GetRanking(guest));
            PlayerMatchRecord record = channel.GetMyRecord(token);
            FaultException<ServiceFault> denied = Assert.Throws<FaultException<ServiceFault>>(
                () => channel.GetMyRecord(guest));
            Assert.Equal(ServiceErrorCodes.Unauthorized, denied.Detail.ErrorCode);
            Assert.Equal(expected.FloorsBuilt, entry.FloorsBuilt);
            Assert.Equal(expected.Won ? 1 : 0, entry.MatchesWon);
            Assert.Equal((1, entry.MatchesWon), (record.MatchesPlayed, record.MatchesWon));

            using IServiceScope scope = server.Services.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<VertigoDatabaseContext>();
            Assert.Equal(expected.HighestSkyscraper, (await database.PlayerStats.SingleAsync()).HighestSkyscraper);
        }
        finally
        {
            ((IClientChannel)channel).Abort();
            factory.Abort();
        }
    }

    private void VerifyUpdates(PlayerChannels players, MatchIdentity match, Dictionary<Guid, MatchSyncState> baselines)
    {
        foreach (PlayerChannel player in players.All)
        {
            Guid playerId = player.Session.PlayerId;
            MatchSyncState baseline = baselines[playerId];
            (MatchSyncState merged, TransferSizes sizes) = VerifyPlayerUpdate(player, match, baseline);
            baselines[playerId] = merged;
            if (player == players.Host)
            {
                _output.WriteLine($"{players.All.Count} jugadores, rondas cerradas: {merged.Board.RoundSummaries.Count}, "
                    + $"revisión {baseline.Board.Revision} → {merged.Board.Revision}: "
                    + $"completa {sizes.Full}, incremental {sizes.Delta}, sin cambios {sizes.Unchanged} bytes XML.");
            }
        }
    }

    private static (MatchSyncState State, TransferSizes Sizes) VerifyPlayerUpdate(
        PlayerChannel player, MatchIdentity match, MatchSyncState baseline)
    {
        ILobbyService channel = player.Channel;
        Guid playerId = player.Session.PlayerId;
        MatchSyncRequest initial = Request(match, player.Session);
        MatchSyncRequest incremental = Request(match, player.Session);
        incremental.Known = MatchSynchronization.Describe(baseline);

        MatchSyncUpdate full = channel.GetMatchUpdate(initial);
        MatchSyncUpdate delta = channel.GetMatchUpdate(incremental);
        MatchPrivateHand privateHand = channel.GetMatchPrivateHand(match.LobbyCode, player.Session.Token);
        MatchSyncState merged = MatchSynchronization.Apply(delta, baseline, playerId);
        MatchSyncState expected = MatchSynchronization.Apply(full, null, playerId);
        Assert.Equal(match.MatchId, merged.Board.MatchId);
        Assert.Equal(baseline.Board.Revision, delta.BaseRevision);
        Assert.Equal(playerId, full.PlayerId);
        Assert.Equal(playerId, delta.PlayerId);
        Assert.Equal(playerId, privateHand.PlayerId);
        Assert.Equal(merged.Board.Revision, privateHand.Revision);
        Assert.Equivalent(expected.Board, merged.Board, strict: true);
        Assert.Equivalent(privateHand, expected.Hand, strict: true);
        Assert.Equivalent(privateHand, merged.Hand, strict: true);

        long deltaBytes = Bytes(incremental) + Bytes(delta);
        incremental.Known = MatchSynchronization.Describe(merged);
        MatchSyncUpdate unchanged = channel.GetMatchUpdate(incremental);
        Assert.Equal(playerId, unchanged.PlayerId);
        Assert.Equal(merged.Board.Revision, unchanged.BaseRevision);
        Assert.Equal(merged.Board.Revision, unchanged.Revision);
        Assert.Null(unchanged.Piles);
        Assert.Null(unchanged.Players);
        Assert.Null(unchanged.RecentMoves);
        Assert.Null(unchanged.Rounds);
        Assert.Null(unchanged.Status);
        Assert.False(unchanged.HandChanged);
        Assert.Null(unchanged.Hand);
        MatchSyncState sameRevision = MatchSynchronization.Apply(unchanged, merged, playerId);
        Assert.Equivalent(merged.Board, sameRevision.Board, strict: true);
        Assert.Equivalent(privateHand, sameRevision.Hand, strict: true);
        long completeBytes = Bytes(initial) + Bytes(full);
        long unchangedBytes = Bytes(incremental) + Bytes(unchanged);
        Assert.True(unchangedBytes < completeBytes);

        return (merged, new TransferSizes(completeBytes, deltaBytes, unchangedBytes));
    }

    private static MatchSyncRequest Request(MatchIdentity match, AuthenticatedSession session)
    {
        return new MatchSyncRequest
        {
            LobbyCode = match.LobbyCode,
            MatchId = match.MatchId,
            SessionToken = session.Token
        };
    }

    private static MatchBoardSnapshot PlayTurn(PlayerChannel active, string lobbyCode, MatchBoardSnapshot board)
    {
        string token = active.Session.Token;
        MatchPrivateHand hand = active.Channel.GetMatchPrivateHand(lobbyCode, token);
        (int card, int pile) = (from tower in board.Piles
            from candidate in hand.Cards
            where tower.TopCardNumber is int top && MatchPlacementRules.IsValid(top, candidate)
            select (candidate, tower.Position)).FirstOrDefault();

        MatchBoardSnapshot updated;
        if (pile == 0)
        {
            updated = active.Channel.DismantleMatchTower(new MatchDismantleRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                PilePosition = board.Piles.First(tower => tower.TopCardNumber.HasValue).Position,
                ExpectedRevision = board.Revision
            });
        }
        else
        {
            updated = active.Channel.PlaceMatchCard(new MatchPlaceRequest
            {
                LobbyCode = lobbyCode,
                SessionToken = token,
                CardNumber = card,
                PilePosition = pile,
                ExpectedRevision = board.Revision
            });
        }

        Assert.True(updated.Revision > board.Revision);
        return updated;
    }

    private static long Bytes<T>(T model)
    {
        using MemoryStream stream = new();
        new DataContractSerializer(typeof(T)).WriteObject(stream, model);
        return stream.Length;
    }

    private sealed class PlayerChannels : IDisposable
    {
        private readonly DuplexChannelFactory<ILobbyService> _factory;
        private readonly List<PlayerChannel> _players = new();

        public PlayerChannels(LobbyHarness server, int playerCount, AuthenticatedSession? host = null)
        {
            _factory = new DuplexChannelFactory<ILobbyService>(
                new InstanceContext(new Callbacks()), server.Binding, server.Address);
            for (int index = 0; index < playerCount; index++)
            {
                AuthenticatedSession session = index == 0 && host is not null ? host : server.Guests.Create();
                _players.Add(new PlayerChannel(session, _factory.CreateChannel()));
            }
        }

        public IReadOnlyList<PlayerChannel> All => _players;

        public PlayerChannel Host => _players[0];

        public PlayerChannel For(Guid playerId)
        {
            return _players.Single(player => player.Session.PlayerId == playerId);
        }

        public MatchIdentity StartMatch()
        {
            string lobby = Host.Channel.CreateLobby(true, _players.Count, Host.Session.Token);
            foreach (PlayerChannel player in _players.Skip(1))
            {
                player.Channel.JoinLobby(lobby, player.Session.Token);
            }

            foreach (PlayerChannel player in _players)
            {
                player.Channel.SetReady(lobby, true, player.Session.Token);
            }

            LobbyPreparation started = Host.Channel.StartMatch(lobby, Host.Session.Token);
            return new MatchIdentity(lobby, started.MatchId);
        }

        public void Dispose()
        {
            foreach (PlayerChannel player in _players)
            {
                ((IClientChannel)player.Channel).Abort();
            }

            _factory.Abort();
        }
    }

    private sealed record MatchIdentity(string LobbyCode, Guid MatchId);

    private sealed record TransferSizes(long Full, long Delta, long Unchanged);

    private sealed record PlayerChannel(AuthenticatedSession Session, ILobbyService Channel);

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return now;
        }
    }

    private sealed class Callbacks : ILobbyCallback
    {
        public void Notify(LobbyNotification notification)
        {
        }
    }
}
