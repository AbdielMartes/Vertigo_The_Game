using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Vertigo.Server.Matchmaking.Chat.Services;

public sealed class ChatLanguageFilter
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(250);

    private static readonly string[] DefaultTerms =
    [
        "cabrón", "cabrona", "cabrones", "cabronas", "chinga", "chingada", "chingado", "chíngate", "coño",
        "estúpido", "estúpida", "gilipollas", "hijo de perra", "hija de perra", "idiota", "idiotas", "imbécil",
        "imbéciles", "mierda",
        "pendejo", "pendeja", "pendejos", "pendejas", "puta", "puto", "putas", "putos", "verga",
        "asshole", "assholes", "bitch", "bitches", "bullshit", "fuck", "fucker", "fucking", "motherfucker", "shit"
    ];

    private static readonly Regex SeparatorPattern = new(
        @"[^\p{L}\p{Nd}]+", RegexOptions.CultureInvariant, RegexTimeout);
    private static readonly Regex SpelledWordPattern = new(
        @"(?<![\p{L}\p{Nd}])[\p{L}\p{Nd}](?:[\p{P}\p{S}]+[\p{L}\p{Nd}](?![\p{L}\p{Nd}]))+",
        RegexOptions.CultureInvariant, RegexTimeout);

    private readonly string[] _blockedTerms;

    public ChatLanguageFilter(IEnumerable<string>? terms = null)
    {
        string[] configuredTerms = (terms ?? []).Select(NormalizeTerm)
            .Where(term => term.Length > 0).Distinct(StringComparer.Ordinal).ToArray();
        _blockedTerms = (configuredTerms.Length > 0 ? configuredTerms : DefaultTerms.Select(NormalizeTerm))
            .Select(WithWordBoundaries).ToArray();
    }

    public bool ContainsBlockedLanguage(string message)
    {
        string spelling = NormalizeSpelling(message);
        return MatchesBlockedTerm(spelling) || MatchesAlternateSpelling(spelling);
    }

    private bool MatchesAlternateSpelling(string spelling)
    {
        string obfuscatedSpelling = string.Concat(spelling.Select(ReplaceDigit));
        bool blocked = spelling != obfuscatedSpelling && MatchesBlockedTerm(obfuscatedSpelling);
        if (!blocked)
        {
            string joined = SpelledWordPattern.Replace(obfuscatedSpelling,
                match => SeparatorPattern.Replace(match.Value, string.Empty));
            blocked = joined != obfuscatedSpelling && MatchesBlockedTerm(joined);
        }

        return blocked;
    }

    private bool MatchesBlockedTerm(string spelling)
    {
        string normalizedMessage = WithWordBoundaries(SeparatorPattern.Replace(spelling, " ").Trim());
        return _blockedTerms.Any(term => normalizedMessage.Contains(term, StringComparison.Ordinal));
    }

    private static string NormalizeTerm(string text)
    {
        return SeparatorPattern.Replace(NormalizeSpelling(text), " ").Trim();
    }

    private static string NormalizeSpelling(string text)
    {
        string decomposed = text.Normalize(NormalizationForm.FormKD).ToLowerInvariant()
            .Replace("n\u0303", "ñ", StringComparison.Ordinal);
        return string.Concat(decomposed.Where(character => char.GetUnicodeCategory(character)
            is not (UnicodeCategory.NonSpacingMark or UnicodeCategory.Format)));
    }

    private static char ReplaceDigit(char character) => character switch
    {
        '0' => 'o',
        '1' => 'i',
        '3' => 'e',
        '4' => 'a',
        '5' => 's',
        '7' => 't',
        _ => character
    };

    private static string WithWordBoundaries(string text) => $" {text} ";
}
