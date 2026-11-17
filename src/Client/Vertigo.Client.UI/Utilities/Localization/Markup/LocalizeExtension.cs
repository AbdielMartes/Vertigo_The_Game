using System;
using Avalonia.Markup.Xaml;
using Vertigo.Client.UI.Utilities.Localization.Services;

namespace Vertigo.Client.UI.Utilities.Localization.Markup;

public sealed class LocalizeExtension : MarkupExtension
{
    private static readonly TextLocalizer Localizer = new();

    public LocalizeExtension(string key)
    {
        Key = key;
    }

    public string Key { get; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        string value = Localizer.Get(Key);
        return value;
    }
}
