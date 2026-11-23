namespace Vertigo.Contracts.Models;

public static class MatchChatRules
{
    public const int MaximumLength = 30;

    public static bool CanSend(string? message)
    {
        return !string.IsNullOrWhiteSpace(message) && message.Length <= MaximumLength
            && message.AsSpan().IndexOfAny("\r\n\u0085\u2028\u2029") < 0;
    }
}
