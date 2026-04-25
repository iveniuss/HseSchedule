using System.Globalization;
using System.Text;
using Shared.Models;

namespace ScheduleAPI.Services;

public class CalendarService
{
    /// <summary>
    /// Creates iCal calendar with given lessons
    /// </summary>
    /// <param name="lessons">List of lessons</param>
    /// <returns>iCal string with given lessons</returns>
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
            builder.AppendLine($"UID:{lesson.Id}@scheduleapi");
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

    /// <summary>
    /// Creates date string for iCal
    /// </summary>
    /// <param name="dateTime">event dateTime in UTC</param>
    /// <returns>string in iCal supported format</returns>
    private static string FormatDateTimeUtc(DateTime dateTime) =>
        dateTime.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);


    /// <summary>
    /// Escapes special characters in a string for use in an iCal file.
    /// </summary>
    /// <param name="value">The input string to escape.</param>
    /// <returns>A string with special characters escaped for iCal compatibility.</returns>
    private static string EscapeIcalText(string value) =>
        value
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n")
            .Replace("\r", "\\n");
}