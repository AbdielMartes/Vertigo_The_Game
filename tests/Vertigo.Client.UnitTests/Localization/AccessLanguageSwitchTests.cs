using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class AccessLanguageSwitchTests
{
    [Theory]
    [InlineData("es", "Bienvenidos", "Registrarse")]
    [InlineData("en", "Welcome", "Register")]
    public void Get_AccessLabels_ReturnsExpectedCulture(string cultureName, string title, string register)
    {
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            Assert.Equal(title, localizer.Get("Access.Title"));
            Assert.Equal(register, localizer.Get("Access.Register"));
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
