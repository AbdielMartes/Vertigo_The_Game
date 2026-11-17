using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchPlacementRulesTests
{
    [Theory]
    [InlineData(40, 41)]
    [InlineData(40, 50)]
    [InlineData(1, 11)]
    public void IsValid_AllowedGap_ReturnsTrue(int top, int card)
    {
        bool valid = MatchPlacementRules.IsValid(top, card);
        Assert.True(valid);
    }

    [Theory]
    [InlineData(40, 40)]
    [InlineData(40, 39)]
    [InlineData(40, 51)]
    [InlineData(100, 101)]
    [InlineData(0, 5)]
    [InlineData(45, 0)]
    public void IsValid_ForbiddenGap_ReturnsFalse(int top, int card)
    {
        bool valid = MatchPlacementRules.IsValid(top, card);
        Assert.False(valid);
    }
}
