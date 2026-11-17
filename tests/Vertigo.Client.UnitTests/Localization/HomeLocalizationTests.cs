using System.Globalization;
using Vertigo.Client.UI.Utilities.Localization.Services;
using Xunit;

namespace Vertigo.Client.UnitTests.Localization;

public sealed class HomeLocalizationTests
{
    [Theory]
    [InlineData("es-MX")]
    [InlineData("en-US")]
    public void Get_HomeKeys_AreLocalized(string cultureName)
    {
        CultureInfo previous = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);
            TextLocalizer localizer = new();
            string[] keys =
            [
                "Home.BrandSubtitle", "Home.Play", "Home.Social",
                "Home.Ranking", "Home.Settings", "Home.Profile",
                "Home.Notifications", "Home.Logout", "Home.Create",
                "Home.Join", "Home.Glossary", "Home.GuestAccount",
                "Home.RegisteredAccount", "Home.NotificationsHelp"
            ];

            foreach (string key in keys)
            {
                string actual = localizer.Get(key);
                Assert.False(string.IsNullOrWhiteSpace(actual));
                Assert.NotEqual(key, actual);
            }
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
