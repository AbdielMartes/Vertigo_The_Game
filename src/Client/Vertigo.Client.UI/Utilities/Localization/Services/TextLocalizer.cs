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
        if (string.IsNullOrWhiteSpace(key))
        {
            return _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture) ?? "Ocurrió un error.";
        }

        return _resources.GetString(key, CultureInfo.CurrentUICulture)
            ?? _resources.GetString("Error.Unknown", CultureInfo.CurrentUICulture)
            ?? "Ocurrió un error.";
    }
}
