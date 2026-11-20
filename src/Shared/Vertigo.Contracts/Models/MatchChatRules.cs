namespace Vertigo.Contracts.Models;

public static class MatchChatRules
{
    public const int MaximumLength = 30;

    public static bool CanSend(string? message)
    {
        return !string.IsNullOrWhiteSpace(message) && message.Length <= MaximumLength
            && !message.Contains('\r') && !message.Contains('\n');
    }
}
