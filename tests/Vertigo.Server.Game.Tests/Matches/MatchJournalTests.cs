using Vertigo.Contracts.Internal;
using Vertigo.Contracts.Models;
using Vertigo.Server.CrossCutting.Recovery;
using Vertigo.Server.Game.Matches.Implementations;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchJournalTests
{
    [Fact]
    public void TryExpireTurn_WriteFails_RollsBackAndCanRetry()
    {
        DirectoryInfo storage = Directory.CreateTempSubdirectory();
        try
        {
            using RecoveryDirectory directory = new(storage.FullName);
            ControlledClock clock = new();
            InMemoryMatchSessionStarter game = new(clock, directory);
            Guid host = Guid.NewGuid();
            Guid matchId = game.Start(new MatchStartRequest
            {
                HostPlayerId = host,
                LobbyCode = "TESTAB",
                LobbyInstanceId = Guid.NewGuid(),
                PlayerIds = new[] { host, Guid.NewGuid() }
            }).MatchId;

            MatchBoardSnapshot before = game.GetBoard(matchId)!;
            string matchPath = Path.Combine(storage.FullName, "matches-v1", matchId.ToString("N"));
            Assert.Null(game.TryExpireTurn(matchId));
            Assert.Empty(Directory.GetFiles(matchPath, "*.event.json"));
            clock.Now = before.TurnDeadlineUtc!.Value;
            string blocked = Path.Combine(matchPath, "0000000001.event.json.pending");
            Directory.CreateDirectory(blocked);

            Exception? failure = Record.Exception(() => game.TryExpireTurn(matchId));

            Assert.True(failure is IOException or UnauthorizedAccessException);
            MatchBoardSnapshot afterFailure = game.GetBoard(matchId)!;
            Assert.Equal(before.Revision, afterFailure.Revision);
            Assert.Equal(before.CurrentTurnPlayerId, afterFailure.CurrentTurnPlayerId);
            Assert.Empty(Directory.GetFiles(matchPath, "*.event.json"));

            Directory.Delete(blocked);
            Assert.Equal(before.CurrentTurnPlayerId, game.TryExpireTurn(matchId));
            Assert.Single(Directory.GetFiles(matchPath, "*.event.json"));
        }
        finally
        {
            storage.Delete(recursive: true);
        }
    }

    private sealed class ControlledClock : TimeProvider
    {
        public DateTimeOffset Now { get; set; } = DateTimeOffset.UnixEpoch;

        public override DateTimeOffset GetUtcNow()
        {
            return Now;
        }
    }
}
