using Vertigo.Client.UI.ViewModels.Lifecycle;
using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Matches;

public sealed class MatchEntryRulesTests
{
    [Fact]
    public void CanEnter_ValidStartedMatch_ReturnsTrue()
    {
        LobbyPreparation preparation = CreatePreparation(true, Guid.NewGuid());

        bool result = MatchEntryRules.CanEnter(preparation, "ABC234");

        Assert.True(result);
    }

    [Fact]
    public void CanEnter_MissingMatchId_ReturnsFalse()
    {
        LobbyPreparation preparation = CreatePreparation(true, Guid.Empty);

        bool result = MatchEntryRules.CanEnter(preparation, "ABC234");

        Assert.False(result);
    }

    [Fact]
    public void CanEnter_NotStarted_ReturnsFalse()
    {
        LobbyPreparation preparation = CreatePreparation(false, Guid.NewGuid());

        bool result = MatchEntryRules.CanEnter(preparation, "ABC234");

        Assert.False(result);
    }

    [Fact]
    public void CanEnter_AnotherLobby_ReturnsFalse()
    {
        LobbyPreparation preparation = CreatePreparation(true, Guid.NewGuid());

        bool result = MatchEntryRules.CanEnter(preparation, "XYZ234");

        Assert.False(result);
    }

    [Fact]
    public void CanEnter_EmptyLobbyCode_ReturnsFalse()
    {
        LobbyPreparation preparation = CreatePreparation(true, Guid.NewGuid());

        bool result = MatchEntryRules.CanEnter(preparation, string.Empty);

        Assert.False(result);
    }

    private static LobbyPreparation CreatePreparation(bool isInGame, Guid matchId)
    {
        LobbyPreparation preparation = new()
        {
            LobbyCode = "ABC234",
            IsInGame = isInGame,
            MatchId = matchId,
            Revision = 4
        };

        return preparation;
    }
}
