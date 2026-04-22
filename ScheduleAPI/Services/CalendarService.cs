using System.Globalization;
using System.Text;
using Shared.Models;

namespace ScheduleAPI.Services;

public class CalendarService
{
    public string GetIcal(List<Lesson> lessons)
    {
        var builder = new StringBuilder();

        builder.AppendLine("BEGIN:VCALENDAR");
        builder.AppendLine("VERSION:2.0");
        builder.AppendLine("PRODID:-//ScheduleAPI//Schedule Calendar v1.1//EN");
        builder.AppendLine("X-WR-CALNAME:Расписание");
        builder.AppendLine("X-WR-TIMEZONE:Asia/Yekaterinburg");
        builder.AppendLine("CALSCALE:GREGORIAN");

        foreach (var lesson in lessons.OrderBy(x => x.StartTime))
        {
            builder.AppendLine("BEGIN:VEVENT");
            builder.AppendLine($"UID:{lesson.Id}-{lesson.StartTime:yyyyMMddTHHmmss}@scheduleapi");
            builder.AppendLine($"DTSTAMP:{FormatDateTimeUtc(lesson.LastUpdate)}");
            builder.AppendLine($"DTSTART:{FormatDateTimeUtc(lesson.StartTime)}");
            builder.AppendLine($"DTEND:{FormatDateTimeUtc(lesson.EndTime)}");
            builder.AppendLine($"SUMMARY:{EscapeIcalText(lesson.Name)}");

            if (!string.IsNullOrWhiteSpace(lesson.Location))
            {
                builder.AppendLine($"LOCATION:{EscapeIcalText(lesson.Location)}");
            }

            var descriptionParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(lesson.Teacher))
            {
                descriptionParts.Add($"{lesson.Teacher}");
            }

            if (!string.IsNullOrWhiteSpace(lesson.Link))
            {
                descriptionParts.Add($"Ссылка: {lesson.Link}");
            }

            if (descriptionParts.Count > 0)
            {
                builder.AppendLine($"DESCRIPTION:{EscapeIcalText(string.Join("\n", descriptionParts))}");
            }

            builder.AppendLine("END:VEVENT");
        }
        builder.AppendLine("END:VCALENDAR");

        return builder.ToString();
    }

    private static string FormatDateTimeUtc(DateTime dateTime) =>
        dateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);

    private static string EscapeIcalText(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
}