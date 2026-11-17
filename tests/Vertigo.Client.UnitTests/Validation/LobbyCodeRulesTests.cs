using Vertigo.Client.UI.Utilities.Validation;
using Xunit;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class LobbyCodeRulesTests
{
    [Theory]
    [InlineData("ABC234", "ABC234")]
    [InlineData(" abc234 ", "ABC234")]
    [InlineData("Z9K8T7", "Z9K8T7")]
    public void TryNormalize_ValidCode_ReturnsNormalized(string input, string expected)
    {
        bool result = LobbyCodeRules.TryNormalize(input, out string code);

        Assert.True(result);
        Assert.Equal(expected, code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC23")]
    [InlineData("ABC2345")]
    [InlineData("ABC_34")]
    [InlineData("ABO234")]
    [InlineData("ABI234")]
    [InlineData("AB1234;")]
    public void TryNormalize_InvalidCode_ReturnsFalse(string input)
    {
        bool result = LobbyCodeRules.TryNormalize(input, out string code);

        Assert.False(result);
        Assert.Empty(code);
    }
}
