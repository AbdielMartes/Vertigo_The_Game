using System.Globalization;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed record RankingRow(int Position, string Username, int MatchesWon, int FloorsBuilt)
{
    public string Initial => string.IsNullOrWhiteSpace(Username)
        ? "?" : StringInfo.GetNextTextElement(Username).ToUpper(CultureInfo.CurrentCulture);
}
