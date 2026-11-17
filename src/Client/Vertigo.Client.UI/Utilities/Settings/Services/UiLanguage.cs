using System.Globalization;

namespace Vertigo.Client.UI.Utilities.Settings.Services;

public static class UiLanguage
{
    public static void Apply(string language)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(language == "en" ? "en" : "es");
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
