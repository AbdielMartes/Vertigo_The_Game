namespace Vertigo.Client.UI.Utilities.Validation;

public static class LobbyCodeRules
{
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static bool TryNormalize(string? input, out string code)
    {
        string candidate = (input ?? string.Empty).Trim().ToUpperInvariant();
        bool isValid = candidate.Length == 6;

        if (isValid)
        {
            foreach (char character in candidate)
            {
                if (!Alphabet.Contains(character))
                {
                    isValid = false;
                    break;
                }
            }
        }

        code = isValid ? candidate : string.Empty;
        return isValid;
    }
}
