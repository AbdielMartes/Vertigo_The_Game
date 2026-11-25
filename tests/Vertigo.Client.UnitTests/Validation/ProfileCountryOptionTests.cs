using Vertigo.Client.UI.ViewModels.Presentation;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class ProfileCountryOptionTests
{
    [Theory]
    [InlineData(" mexico ", true)]
    [InlineData("mx", true)]
    [InlineData("México (MX)", true)]
    [InlineData("Méx", false)]
    [InlineData("MEX", false)]
    public void MatchesExactly_RequiresACompleteNameOrStoredCode(string text, bool expected)
    {
        ProfileCountryOption option = new("MX", "México");

        Assert.Equal(expected, option.MatchesExactly(text));
        Assert.True(option.Matches("mex"));
        Assert.Equal("MX", option.Code);
    }

    [Fact]
    public void MatchesExactly_OnlyUnspecifiedAcceptsEmptyText()
    {
        Assert.True(new ProfileCountryOption(null, "Sin especificar").MatchesExactly(" "));
        Assert.False(new ProfileCountryOption("ZA", "Sudáfrica").MatchesExactly(" "));
    }
}
