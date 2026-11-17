using Vertigo.Client.UI.Utilities.Validation;
using Xunit;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class MatchChatRulesTests
{
    [Fact]
    public void CanSend_ExactlyFortyCharacters_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend(new string('a', 40)));
    }

    [Fact]
    public void CanSend_FortyOneCharacters_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend(new string('a', 41)));
    }

    [Fact]
    public void CanSend_WhitespaceOnly_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend("   "));
    }

    [Fact]
    public void CanSend_ValidShortMessage_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend("¡Vamos!"));
    }
}
