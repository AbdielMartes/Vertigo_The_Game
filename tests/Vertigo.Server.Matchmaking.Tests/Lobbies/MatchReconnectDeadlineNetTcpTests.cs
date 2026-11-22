using System.ServiceModel;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Callbacks;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class MatchReconnectDeadlineNetTcpTests
{
    [Theory]
    [InlineData(-1L, true)]
    [InlineData(0L, false)]
    [InlineData(1L, false)]
    public async Task ReconnectMatch_DisconnectedDuringTurn_EnforcesGraceDeadline(long offset, bool canReconnect)
    {
        MatchClock clock = new();
        await using LobbyHarness server = await LobbyHarness.StartAsync(clock: clock);
        using DuplexChannelFactory<ILobbyService> channels = new(
            new InstanceContext(new LobbyCallback()), server.Binding, server.Address);
        Player[] players = Enumerable.Range(0, 3)
            .Select(_ => new Player(server.Guests.Create(), channels.CreateChannel())).ToArray();

        try
        {
            string code = StartMatch(players);
            MatchBoardSnapshot started = players[0].Channel.GetMatchBoard(code, players[0].Session.Token);
            Player target = players.Single(player => player.Session.PlayerId == started.CurrentTurnPlayerId);
            Player observer = players.First(player => player != target);
            MatchPrivateHand original = target.Channel.GetMatchPrivateHand(code, target.Session.Token);
            ((IClientChannel)target.Channel).Abort();
            await WaitForDisconnectAsync(observer, code, target.Session.PlayerId);

            LobbyService lobby = server.Services.GetRequiredService<LobbyService>();
            DateTimeOffset turnDeadline = started.TurnDeadlineUtc!.Value;
            clock.Set(turnDeadline.AddTicks(-1));
            lobby.ProcessExpiredMatchTurns();
            Assert.Equal(started.Revision, observer.Channel.GetMatchBoard(code, observer.Session.Token).Revision);

            clock.Set(turnDeadline);
            lobby.ProcessExpiredMatchTurns();
            MatchBoardSnapshot skipped = observer.Channel.GetMatchBoard(code, observer.Session.Token);
            Assert.Equal(MatchBoardActionKind.TurnSkipped, skipped.RecentMoves.Last().ActionKind);
            Assert.NotEqual(target.Session.PlayerId, skipped.CurrentTurnPlayerId);
            DateTimeOffset graceDeadline = skipped.PresenceDeadlinesUtc[target.Session.PlayerId];
            Assert.Equal(turnDeadline.AddSeconds(30), graceDeadline);
            clock.Set(graceDeadline.AddTicks(offset));
            ILobbyService reconnecting = channels.CreateChannel();
            try
            {
                MatchPresenceRequest request = new()
                {
                    LobbyCode = code, MatchId = started.MatchId, SessionToken = target.Session.Token
                };
                if (canReconnect)
                {
                    VerifyAccepted(reconnecting, request, original);
                }
                else
                {
                    VerifyRejected(reconnecting, request);
                }

                clock.Set(graceDeadline.AddTicks(Math.Max(offset, 0)));
                lobby.ProcessExpiredMatchTurns();
                MatchBoardSnapshot after = observer.Channel.GetMatchBoard(code, observer.Session.Token);
                Assert.Equal(started.MatchId, after.MatchId);
                Assert.Equal(MatchEndReason.None, after.EndReason);
                Assert.Equal(!canReconnect, after.ExpelledPlayerIds.Contains(target.Session.PlayerId));
                Assert.Equal(canReconnect ? 3 : 2, after.Players.Count);
                if (!canReconnect)
                {
                    Assert.Equal(MatchBoardActionKind.TurnExpired, after.RecentMoves.Last().ActionKind);
                    VerifyExpelled(reconnecting, request, target.Session.PlayerId);
                }
            }
            finally
            {
                ((IClientChannel)reconnecting).Abort();
            }
        }
        finally
        {
            foreach (Player player in players)
            {
                ((IClientChannel)player.Channel).Abort();
            }

            channels.Abort();
        }
    }

    private static void VerifyAccepted(ILobbyService channel, MatchPresenceRequest request, MatchPrivateHand original)
    {
        Assert.Equal(request.LobbyCode, channel.ReconnectMatch(request).LobbyCode);
        MatchPrivateHand resumed = channel.GetMatchPrivateHand(request.LobbyCode, request.SessionToken);
        Assert.Equal(original.PlayerId, resumed.PlayerId);
        Assert.Equal(original.Cards, resumed.Cards);
        MatchBoardSnapshot confirmed = channel.ConfirmMatchPresence(request);
        Assert.Equal(request.MatchId, confirmed.MatchId);
        Assert.DoesNotContain(original.PlayerId, confirmed.PresenceDeadlinesUtc.Keys);
    }

    private static void VerifyRejected(ILobbyService channel, MatchPresenceRequest request)
    {
        FaultException<ServiceFault> fault = Assert.Throws<FaultException<ServiceFault>>(
            () => channel.ReconnectMatch(request));
        Assert.Equal(ServiceErrorCodes.LobbyNotFound, fault.Detail.ErrorCode);
        Assert.Equal(ServiceMessageKeys.LobbyNotFound, fault.Detail.MessageKey);

        FaultException<ServiceFault> joinFault = Assert.Throws<FaultException<ServiceFault>>(
            () => channel.JoinLobby(request.LobbyCode, request.SessionToken));
        Assert.Equal(ServiceErrorCodes.LobbyAlreadyStarted, joinFault.Detail.ErrorCode);
    }

    private static void VerifyExpelled(ILobbyService channel, MatchPresenceRequest request, Guid playerId)
    {
        Assert.Equal(request.LobbyCode, channel.ReconnectMatch(request).LobbyCode);
        MatchBoardSnapshot board = channel.GetMatchBoard(request.LobbyCode, request.SessionToken);
        MatchPrivateHand hand = channel.GetMatchPrivateHand(request.LobbyCode, request.SessionToken);
        Assert.Equal(request.MatchId, board.MatchId);
        Assert.Contains(playerId, board.InactivePlayerIds);
        Assert.DoesNotContain(board.Players, player => player.PlayerId == playerId);
        Assert.Empty(hand.Cards);

        MatchSyncUpdate update = channel.GetMatchUpdate(new MatchSyncRequest
        {
            MatchId = request.MatchId,
            LobbyCode = request.LobbyCode,
            SessionToken = request.SessionToken
        });
        MatchSyncState synced = MatchSynchronization.Apply(update, null, playerId);
        Assert.Equal(request.MatchId, synced.Board.MatchId);
        Assert.Contains(playerId, synced.Board.InactivePlayerIds);
        Assert.Null(synced.Hand);
    }

    private static string StartMatch(Player[] players)
    {
        Player host = players[0];
        string code = host.Channel.CreateLobby(true, players.Length, host.Session.Token);
        foreach (Player player in players.Skip(1))
        {
            player.Channel.JoinLobby(code, player.Session.Token);
        }

        foreach (Player player in players)
        {
            player.Channel.SetReady(code, true, player.Session.Token);
        }

        Assert.NotEqual(Guid.Empty, host.Channel.StartMatch(code, host.Session.Token).MatchId);
        return code;
    }

    private static async Task WaitForDisconnectAsync(Player observer, string code, Guid playerId)
    {
        using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(5));
        while (!observer.Channel.GetPreparation(code, observer.Session.Token).DisconnectedPlayerIds.Contains(playerId))
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20), timeout.Token);
        }
    }

    private sealed record Player(AuthenticatedSession Session, ILobbyService Channel);

    private sealed class MatchClock : TimeProvider
    {
        private long _ticks = DateTimeOffset.UtcNow.UtcTicks;

        public override DateTimeOffset GetUtcNow()
        {
            return new DateTimeOffset(Interlocked.Read(ref _ticks), TimeSpan.Zero);
        }

        public void Set(DateTimeOffset time)
        {
            Interlocked.Exchange(ref _ticks, time.UtcTicks);
        }
    }

    private sealed class LobbyCallback : ILobbyCallback
    {
        public void Notify(LobbyNotification notification)
        {
        }
    }
}
