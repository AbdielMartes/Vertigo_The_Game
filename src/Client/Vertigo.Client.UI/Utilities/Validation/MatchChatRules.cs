namespace Vertigo.Client.UI.Utilities.Validation;

public static class MatchChatRules
{
    public const int MaximumLength = 40;

    public static bool CanSend(string? message)
    {
        bool isValid = !string.IsNullOrWhiteSpace(message);

        if (isValid)
        {
            isValid = message!.Length <= MaximumLength;
        }

        return isValid;
    }
}
