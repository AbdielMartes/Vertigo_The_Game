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
        string result = string.Empty;
        if (sentAt != default)
        {

            DateTime utc = sentAt.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(sentAt, DateTimeKind.Utc)
                : sentAt.ToUniversalTime();
            result = TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone).ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        return result;
    }
}
