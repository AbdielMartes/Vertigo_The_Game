using System;
using System.Globalization;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchChatRow
{
    public required string Sender { get; init; }
    public required string Text { get; init; }
    public DateTime SentAt { get; init; }

    public string LocalTimeText => FormatTime(SentAt, TimeZoneInfo.Local);

    internal static string FormatTime(DateTime sentAt, TimeZoneInfo timeZone)
    {
        if (sentAt == default)
        {
            return string.Empty;
        }

        // ChatService supplies UTC. Treat deserialized values without a kind as UTC too.
        DateTime utc = sentAt.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(sentAt, DateTimeKind.Utc)
            : sentAt.ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone).ToString("HH:mm", CultureInfo.InvariantCulture);
    }
}
