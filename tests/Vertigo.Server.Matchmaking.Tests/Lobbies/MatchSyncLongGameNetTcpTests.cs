using System.Runtime.Serialization;
using System.ServiceModel;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Server.Game.Matches.Models;
using Vertigo.Server.PlayerManagement.Authentication.Models;
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
        await using LobbyHarness server = await LobbyHarness.StartAsync(clock: new FixedClock());
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

        public PlayerChannels(LobbyHarness server, int playerCount)
        {
            _factory = new DuplexChannelFactory<ILobbyService>(
                new InstanceContext(new Callbacks()), server.Binding, server.Address);
            for (int index = 0; index < playerCount; index++)
            {
                _players.Add(new PlayerChannel(server.Guests.Create(), _factory.CreateChannel()));
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

    private sealed class FixedClock : TimeProvider
    {
        public override DateTimeOffset GetUtcNow()
        {
            return DateTimeOffset.UnixEpoch;
        }
    }

    private sealed class Callbacks : ILobbyCallback
    {
        public void Notify(LobbyNotification notification)
        {
        }
    }
}
