using System.Globalization;
using System.Resources;
using Vertigo.Client.UI.Utilities.Localization.Interfaces;

namespace Vertigo.Client.UI.Utilities.Localization.Services;

public sealed class TextLocalizer : ITextLocalizer
{
    private const string ResourceBaseName = "Vertigo.Client.UI.Resources.Localization.Strings";
    private readonly ResourceManager _resources = new(ResourceBaseName, typeof(TextLocalizer).Assembly);

    public string Get(string key)
    {
        string? translation = null;
        if (!string.IsNullOrWhiteSpace(key))
        {
            translation = _resources.GetString(key, CultureInfo.CurrentUICulture);
        }

        return translation
            ?? _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture)
            ?? "Ocurrió un error.";
    }
}
