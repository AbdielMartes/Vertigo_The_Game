using Vertigo.Server.Game.Matches.Models;
using Xunit;

namespace Vertigo.Server.Game.Tests.Matches;

public sealed class MatchDeckTests
{
    [Fact]
    public void Draw_AllModules_ContainsEachNumberExactlyOnce()
    {
        MatchDeck deck = new();

        IReadOnlyList<int> cards = deck.Draw(MatchDeck.ModuleCount);

        Assert.Equal(100, cards.Count);
        Assert.Equal(Enumerable.Range(1, 100), cards.OrderBy(number => number));
        Assert.Equal(0, deck.RemainingCount);
    }

    [Fact]
    public void Draw_InsufficientCards_ThrowsWithoutConsumingRemainingCards()
    {
        MatchDeck deck = new();
        deck.Draw(99);

        Assert.Throws<InvalidOperationException>(() => deck.Draw(2));
        Assert.Equal(1, deck.RemainingCount);
    }
}
