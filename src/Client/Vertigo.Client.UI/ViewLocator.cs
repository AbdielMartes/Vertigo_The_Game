using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Vertigo.Client.UI.ViewModels;

namespace Vertigo.Client.UI;

/// <summary>
/// Given a view model, returns the corresponding view if possible.
/// </summary>
[RequiresUnreferencedCode(
    "Default implementation of ViewLocator involves reflection which may be trimmed away.",
    Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        Control? result = null;
        if (param is not null)
        {
            string name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            Type? type = Type.GetType(name);
            if (type is null)
            {
                result = new TextBlock { Text = "Not Found: " + name };
            }
            else
            {
                result = (Control?)Activator.CreateInstance(type);
            }

        }

        return result;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
