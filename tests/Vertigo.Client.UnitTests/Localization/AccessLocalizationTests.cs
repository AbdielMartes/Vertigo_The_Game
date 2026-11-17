using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class AccessLocalizationTests
{
    [Theory]
    [InlineData("es-MX", "Iniciar sesión")]
    [InlineData("en-US", "Sign in")]
    public void Get_AccessLogin_ReturnsLocalizedText(string cultureName, string expected)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();

            string actual = localizer.Get("Access.Login");

            Assert.Equal(expected, actual);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }

    [Theory]
    [InlineData("es-MX")]
    [InlineData("en-US")]
    public void Get_AccessKeys_AreNotEmpty(string cultureName)
    {
        CultureInfo previousCulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            string[] keys =
            [
                "Access.Title", "Access.Description", "Access.Username",
                "Access.Password", "Access.Login", "Access.Guest",
                "Access.ResumeSection", "Access.Token", "Access.Resume",
                "Access.Progress", "Access.Help", "Access.VisualCaption",
                "Access.SecurityNote"
            ];

            foreach (string key in keys)
            {
                string value = localizer.Get(key);
                Assert.False(string.IsNullOrWhiteSpace(value));
                Assert.NotEqual(localizer.Get("Error.Unknown"), value);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousCulture;
        }
    }
}
