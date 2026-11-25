using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Vertigo.Client.UI.Resources;

public sealed class AvatarImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvatarCatalog.Resolve(value as int?);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return AvaloniaProperty.UnsetValue;
    }
}
