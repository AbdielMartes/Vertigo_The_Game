using System.Globalization;
using System.Resources;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class TextLocalizer : ITextLocalizer
{
    private const string ResourceBaseName = "Vertigo.Client.UI.Resources.Localization.Strings";
    private static readonly ResourceManager Resources = new(ResourceBaseName, typeof(TextLocalizer).Assembly);

    internal static string? FindTranslation(string key)
    {
        return Resources.GetString(key, CultureInfo.CurrentUICulture);
    }

    public string Get(string key)
    {
        string? translation = null;
        if (!string.IsNullOrWhiteSpace(key))
        {
            translation = FindTranslation(key);
        }

        return translation
            ?? FindTranslation("Error.Unknown")
            ?? "Ocurrió un error.";
    }
}
