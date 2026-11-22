using System.ServiceModel;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Vertigo.Contracts.Faults;
using Vertigo.Contracts.Interfaces;
using Vertigo.Contracts.Models;
using Vertigo.Contracts.Synchronization;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.CrossCutting.Security.Interfaces;
using Vertigo.Server.Matchmaking.Lobbies.Services;
using Vertigo.Server.PlayerManagement.Authentication.Models;

namespace Vertigo.Server.Matchmaking.Tests.Lobbies;

public sealed class MatchRecoveryNetTcpTests
{
    [Fact]
    public async Task ProcessExpiredMatchTurns_WhenJournalWriteFails_RetriesAndProcessesOtherMatches()
    {
        DirectoryInfo storage = Directory.CreateTempSubdirectory();
        try
        {
            using RecoveryDirectory journal = new(storage.FullName);
            ControlledClock clock = new();
            await using LobbyHarness server = await LobbyHarness.StartAsync(recovery: journal, clock: clock);
            (string firstCode, string firstToken, MatchBoardSnapshot first) = StartGuestMatch(server);
            (string secondCode, string secondToken, MatchBoardSnapshot second) = StartGuestMatch(server);
            clock.Now = first.TurnDeadlineUtc!.Value;
            string matchPath = Path.Combine(storage.FullName, "matches-v1", first.MatchId.ToString("N"));
            string blocked = Path.Combine(matchPath, "0000000001.event.json.pending");
            Directory.CreateDirectory(blocked);
            LobbyService lobbies = server.Services.GetRequiredService<LobbyService>();

            lobbies.ProcessExpiredMatchTurns();

            Assert.Equal(first.Revision, server.Channel.GetMatchBoard(firstCode, firstToken).Revision);
            Assert.True(server.Channel.GetMatchBoard(secondCode, secondToken).Revision > second.Revision);
            Directory.Delete(blocked);
            lobbies.ProcessExpiredMatchTurns();

            Assert.True(server.Channel.GetMatchBoard(firstCode, firstToken).Revision > first.Revision);
            Assert.Single(Directory.GetFiles(matchPath, "*.event.json"));
        }
        finally
        {
            storage.Delete(recursive: true);
        }
    }

    private static (string Code, string Token, MatchBoardSnapshot Board) StartGuestMatch(LobbyHarness server)
    {
        string host = server.Guests.Create().Token;
        string player = server.Guests.Create().Token;
        ILobbyService lobby = server.Channel;
        string code = lobby.CreateLobby(true, 2, host);
        lobby.JoinLobby(code, player);
        lobby.SetReady(code, true, host);
        lobby.SetReady(code, true, player);
        lobby.StartMatch(code, host);
        return (code, host, lobby.GetMatchBoard(code, host));
    }

    private sealed class ControlledClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UtcNow;

        public override DateTimeOffset GetUtcNow()
        {
            return Now;
        }
    }

    [Fact]
    public async Task ReconnectMatch_AfterRestart_RestoresBothOriginalHandsAndTurn()
    {
        DirectoryInfo storage = Directory.CreateTempSubdirectory();
        try
        {
            await VerifyRestartAsync(storage.FullName);
        }
        finally
        {
            storage.Delete(recursive: true);
        }
    }

    private static async Task VerifyRestartAsync(string directory)
    {
        await using SqliteConnection database = new("Data Source=:memory:");
        await database.OpenAsync();
        string guestToken;
        string revokedGuestToken;
        string registeredToken;
        string code;
        MatchBoardSnapshot saved;
        MatchPrivateHand guestHand;
        MatchPrivateHand registeredHand;

        using (RecoveryDirectory journal = new(directory))
        await using (LobbyHarness before = await LobbyHarness.StartAsync(database, journal))
        {
            AuthenticatedSession guest = before.Guests.Create();
            guestToken = guest.Token;
            revokedGuestToken = before.Guests.Create().Token;
            ITokenGenerator tokens = before.Services.GetRequiredService<ITokenGenerator>();
            Assert.True(before.Guests.Revoke(tokens.HashSessionToken(revokedGuestToken)));
            registeredToken = before.SeedRegisteredSession(DateTime.UtcNow.AddMinutes(5), false);
            ILobbyService lobby = before.Channel;
            code = lobby.CreateLobby(true, 2, guestToken);
            lobby.JoinLobby(code, registeredToken);
            lobby.SetReady(code, true, guestToken);
            lobby.SetReady(code, true, registeredToken);
            lobby.StartMatch(code, guestToken);

            MatchBoardSnapshot first = lobby.GetMatchBoard(code, guestToken);
            string openingToken = first.CurrentTurnPlayerId == guest.PlayerId ? guestToken : registeredToken;
            MatchPrivateHand openingHand = lobby.GetMatchPrivateHand(code, openingToken);
            MatchSyncState initial = new(first, openingHand);
            saved = AdvanceTurn(lobby, (first, code), openingToken);
            VerifyChangedBoard(lobby, (code, openingToken), initial);
            guestHand = lobby.GetMatchPrivateHand(code, guestToken);
            registeredHand = lobby.GetMatchPrivateHand(code, registeredToken);
        }

        using RecoveryDirectory retained = new(directory);
        await using LobbyHarness after = await LobbyHarness.StartAsync(database, retained);
        ILobbyService restored = after.Channel;
        FaultException<ServiceFault> revoked = Assert.Throws<FaultException<ServiceFault>>(
            () => restored.GetCurrentLobby(revokedGuestToken));
        Assert.Equal(ServiceErrorCodes.Unauthorized, revoked.Detail.ErrorCode);
        Assert.True(restored.GetPreparation(code, registeredToken).IsSuspended);
        Assert.Null(restored.GetMatchBoard(code, registeredToken).TurnDeadlineUtc);
        string newGuest = after.Guests.Create().Token;
        FaultException<ServiceFault> denied = Assert.Throws<FaultException<ServiceFault>>(
            () => restored.ReconnectMatch(Access(code, saved.MatchId, newGuest)));
        Assert.Equal(ServiceErrorCodes.LobbyNotFound, denied.Detail.ErrorCode);
        restored.ReconnectMatch(Access(code, saved.MatchId, guestToken));
        restored.ReconnectMatch(Access(code, saved.MatchId, registeredToken));

        MatchSyncState baseline = new(saved, guestHand);
        MatchSyncUpdate delta = restored.GetMatchUpdate(new MatchSyncRequest
        {
            MatchId = saved.MatchId,
            LobbyCode = code,
            SessionToken = guestToken,
            Known = MatchSynchronization.Describe(baseline)
        });
        Assert.Equal(saved.Revision, delta.BaseRevision);
        Assert.Null(delta.Piles);
        Assert.Null(delta.Hand);
        MatchSyncState received = MatchSynchronization.Apply(delta, baseline, guestHand.PlayerId);
        MatchBoardSnapshot board = received.Board;
        AssertHand(guestHand, received.Hand!);
        Assert.Equal(saved.MatchId, board.MatchId);
        Assert.Equal(saved.CurrentTurnPlayerId, board.CurrentTurnPlayerId);
        Assert.Equal(saved.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)),
            board.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)));
        Assert.InRange(board.TurnDeadlineUtc!.Value - board.ServerTimeUtc!.Value,
            TimeSpan.FromSeconds(25), TimeSpan.FromSeconds(30));
        Assert.Empty(restored.GetPreparation(code, registeredToken).DisconnectedPlayerIds);
        AssertHand(guestHand, restored.GetMatchPrivateHand(code, guestToken));
        AssertHand(registeredHand, restored.GetMatchPrivateHand(code, registeredToken));
    }

    private static void VerifyChangedBoard(ILobbyService lobby, (string Code, string Token) access,
        MatchSyncState before)
    {
        MatchSyncUpdate update = lobby.GetMatchUpdate(new MatchSyncRequest
        {
            MatchId = before.Board.MatchId,
            LobbyCode = access.Code,
            SessionToken = access.Token,
            Known = MatchSynchronization.Describe(before)
        });

        Assert.NotNull(update.Piles);
        Assert.NotNull(update.RecentMoves);
        MatchSyncState merged = MatchSynchronization.Apply(update, before, before.Hand!.PlayerId);
        MatchBoardSnapshot full = lobby.GetMatchBoard(access.Code, access.Token);
        Assert.Equal(full.Revision, merged.Board.Revision);
        Assert.Equal(full.CurrentTurnPlayerId, merged.Board.CurrentTurnPlayerId);
        Assert.Equal(full.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)),
            merged.Board.Piles.Select(pile => (pile.TopCardNumber, pile.CardCount)));
        AssertHand(lobby.GetMatchPrivateHand(access.Code, access.Token), merged.Hand!);
    }

    private static MatchBoardSnapshot AdvanceTurn(ILobbyService lobby, (MatchBoardSnapshot Board, string Code) match,
        string token)
    {
        MatchBoardSnapshot board = match.Board;
        int top = board.Piles[0].TopCardNumber!.Value;
        int card = lobby.GetMatchPrivateHand(match.Code, token).Cards
            .FirstOrDefault(number => number > top && number - top <= 10);
        MatchBoardSnapshot result;
        if (card > 0)
        {
            result = lobby.PlaceMatchCard(new MatchPlaceRequest
            {
                LobbyCode = match.Code,
                SessionToken = token,
                CardNumber = card,
                ExpectedRevision = board.Revision,
                PilePosition = 1
            });
        }
        else
        {
            result = lobby.DismantleMatchTower(new MatchDismantleRequest
            {
                LobbyCode = match.Code,
                SessionToken = token,
                PilePosition = 1,
                ExpectedRevision = board.Revision
            });
        }

        Assert.True(result.Revision > board.Revision);
        return result;
    }

    private static MatchPresenceRequest Access(string code, Guid matchId, string token)
    {
        return new MatchPresenceRequest { LobbyCode = code, MatchId = matchId, SessionToken = token };
    }

    private static void AssertHand(MatchPrivateHand expected, MatchPrivateHand actual)
    {
        Assert.Equal(expected.PlayerId, actual.PlayerId);
        Assert.Equal(expected.Cards, actual.Cards);
    }
}
