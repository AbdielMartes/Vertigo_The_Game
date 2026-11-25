using System.Globalization;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record ProfileCountryOption(string? Code, string Name)
{
    private const CompareOptions SearchOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    public string DisplayName => Code is null ? Name : $"{Name} ({Code})";

    public bool Matches(string query)
    {
        CompareInfo comparer = CultureInfo.CurrentUICulture.CompareInfo;
        string text = query.Trim();
        return comparer.IndexOf(DisplayName, text, SearchOptions) >= 0
            || comparer.Compare(Code, text, SearchOptions) == 0;
    }

    public bool MatchesExactly(string query)
    {
        CompareInfo comparer = CultureInfo.CurrentUICulture.CompareInfo;
        string text = query.Trim();
        return (text.Length == 0 && Code is null)
            || comparer.Compare(DisplayName, text, SearchOptions) == 0
            || comparer.Compare(Name, text, SearchOptions) == 0
            || comparer.Compare(Code, text, SearchOptions) == 0;
    }

    public override string ToString()
    {
        return DisplayName;
    }
}
