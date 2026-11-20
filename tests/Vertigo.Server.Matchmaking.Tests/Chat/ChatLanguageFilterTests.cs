using Vertigo.Server.Matchmaking.Chat.Services;

namespace Vertigo.Server.Matchmaking.Tests.Chat;

public sealed class ChatLanguageFilterTests
{
    private readonly ChatLanguageFilter _filter = new();

    [Theory]
    [InlineData("MÍERDA")]
    [InlineData("mier\u200Bda")]
    [InlineData("p.u.t.0")]
    [InlineData("¡Chíngate!")]
    [InlineData("hijo, de  perra")]
    [InlineData("SH1T.")]
    [InlineData("co\u006E\u0303o")]
    public void ContainsBlockedLanguage_NormalizesCommonVariations(string text)
    {
        Assert.True(_filter.ContainsBlockedLanguage(text));
    }

    [Theory]
    [InlineData("El diputado colocó su carta.")]
    [InlineData("Pasa por el cono.")]
    [InlineData("We are building a classic tower.")]
    [InlineData("Mi jugada usa 15, 21 y 43.")]
    public void ContainsBlockedLanguage_AllowsSubstringMatchesAndOrdinaryMessages(string text)
    {
        Assert.False(_filter.ContainsBlockedLanguage(text));
    }

    [Fact]
    public void ContainsBlockedLanguage_PreservesNumericTermsAndPhraseBoundaries()
    {
        ChatLanguageFilter custom = new(["torre 51", "a b"]);

        Assert.True(custom.ContainsBlockedLanguage("torre 51"));
        Assert.False(custom.ContainsBlockedLanguage("torre si"));
        Assert.True(custom.ContainsBlockedLanguage("a.b"));
    }

    [Fact]
    public void ContainsBlockedLanguage_AConfiguredDictionaryReplacesDefaults()
    {
        ChatLanguageFilter custom = new(["frase prohibida", " ", "FRASE PROHIBIDA"]);

        Assert.True(custom.ContainsBlockedLanguage("Usó la frase, prohibida."));
        Assert.False(custom.ContainsBlockedLanguage("mierda"));
        Assert.True(new ChatLanguageFilter([" "]).ContainsBlockedLanguage("mierda"));
    }
}
