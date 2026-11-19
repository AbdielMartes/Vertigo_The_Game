using Vertigo.Client.UI.Utilities.Validation;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchBoardNotificationRulesTests
{
    private readonly Guid _matchId = Guid.NewGuid();

    [Fact]
    public void ShouldRefresh_NewerRevisionForCurrentMatch_ReturnsTrue()
    {
        MatchBoardChanged change = Change(3);
        MatchBoardIdentity board = new("ABC234", _matchId);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, 2);

        Assert.True(result);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(1, 2)]
    public void ShouldRefresh_StaleOrDuplicateRevision_ReturnsFalse(long next, long current)
    {
        MatchBoardChanged change = Change(next);
        MatchBoardIdentity board = new("ABC234", _matchId);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, current);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_DifferentMatch_ReturnsFalse()
    {
        MatchBoardChanged change = Change(3);
        MatchBoardIdentity board = new("ABC234", Guid.NewGuid());

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, 2);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_DifferentLobby_ReturnsFalse()
    {
        MatchBoardChanged change = Change(3);
        MatchBoardIdentity board = new("DEF567", _matchId);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, 2);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_SameLobbyDifferentCase_ReturnsTrue()
    {
        MatchBoardChanged change = Change(3);
        MatchBoardIdentity board = new("abc234", _matchId);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, 2);

        Assert.True(result);
    }

    [Fact]
    public void ShouldRefresh_NullNotification_ReturnsFalse()
    {
        MatchBoardIdentity board = new("ABC234", _matchId);

        bool result = MatchBoardNotificationRules.ShouldRefresh(null, board, 2);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_NoActiveMatch_RejectsEvenAMatchingEmptyIdentifier()
    {
        MatchBoardIdentity board = new("ABC234", Guid.Empty);
        MatchBoardChanged change = new()
        {
            LobbyCode = board.LobbyCode,
            MatchId = Guid.Empty,
            Revision = 3,
            PlayerUsername = "Ana"
        };

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, board, 2);

        Assert.False(result);
    }

    private MatchBoardChanged Change(long revision)
    {
        return new MatchBoardChanged { LobbyCode = "ABC234", MatchId = _matchId, Revision = revision, PlayerUsername = "Jugador", CardNumber = 41, PilePosition = 1 };
    }
}
