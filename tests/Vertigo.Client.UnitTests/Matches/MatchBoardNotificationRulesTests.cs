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

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, "ABC234", _matchId, 2);

        Assert.True(result);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(1, 2)]
    public void ShouldRefresh_StaleOrDuplicateRevision_ReturnsFalse(long next, long current)
    {
        MatchBoardChanged change = Change(next);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, "ABC234", _matchId, current);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_DifferentMatch_ReturnsFalse()
    {
        MatchBoardChanged change = Change(3);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, "ABC234", Guid.NewGuid(), 2);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_DifferentLobby_ReturnsFalse()
    {
        MatchBoardChanged change = Change(3);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, "DEF567", _matchId, 2);

        Assert.False(result);
    }

    [Fact]
    public void ShouldRefresh_SameLobbyDifferentCase_ReturnsTrue()
    {
        MatchBoardChanged change = Change(3);

        bool result = MatchBoardNotificationRules.ShouldRefresh(change, "abc234", _matchId, 2);

        Assert.True(result);
    }

    private MatchBoardChanged Change(long revision)
    {
        return new MatchBoardChanged { LobbyCode = "ABC234", MatchId = _matchId, Revision = revision, PlayerUsername = "Jugador", CardNumber = 41, PilePosition = 1 };
    }
}
