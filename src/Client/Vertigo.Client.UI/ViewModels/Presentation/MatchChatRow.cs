using System;
using System.Globalization;
using Vertigo.Contracts.Models;

namespace Vertigo.Client.UI.ViewModels.Presentation;

public sealed class MatchChatRow
{
    public required string Sender { get; init; }
    public required string Text { get; init; }
    public DateTime SentAt { get; init; }
    public bool IsNotice { get; init; }

    public string LocalTimeText => FormatTime(SentAt, TimeZoneInfo.Local);

    internal static MatchChatRow FromMessage(LobbyChatMessage message)
    {
        return new MatchChatRow { Sender = message.SenderUsername, Text = message.Text, SentAt = message.SentAt };
    }

    internal static MatchChatRow Blocked(string sender, string notice)
    {
        return new MatchChatRow { Sender = sender, Text = notice, IsNotice = true, SentAt = DateTime.UtcNow };
    }

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
