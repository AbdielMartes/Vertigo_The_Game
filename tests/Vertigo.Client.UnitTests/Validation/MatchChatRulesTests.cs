using Vertigo.Contracts.Models;
using Xunit;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class MatchChatRulesTests
{
    [Fact]
    public void CanSend_ExactlyThirtyCharacters_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend(new string('a', 30)));
    }

    [Fact]
    public void CanSend_ThirtyOneCharacters_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend(new string('a', 31)));
    }

    [Fact]
    public void CanSend_WhitespaceOnly_ReturnsFalse()
    {
        Assert.False(MatchChatRules.CanSend("   "));
    }

    [Theory]
    [InlineData("Hola\rAdiós")]
    [InlineData("Hola\nAdiós")]
    [InlineData("Hola\u0085Adiós")]
    [InlineData("Hola\u2028Adiós")]
    [InlineData("Hola\u2029Adiós")]
    public void CanSend_MultipleLines_ReturnsFalse(string message)
    {
        Assert.False(MatchChatRules.CanSend(message));
    }

    [Fact]
    public void CanSend_ValidShortMessage_ReturnsTrue()
    {
        Assert.True(MatchChatRules.CanSend("¡Vamos!"));
    }
}
