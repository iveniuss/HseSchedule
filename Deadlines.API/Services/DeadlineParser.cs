using Deadlines.API.Models;
using Ical.Net;
using System.Text.RegularExpressions;

namespace Deadlines.API.Services;

public class DeadlineParser(HttpClient httpClient, CourseNamesProvider courseNamesProvider)
{
    async Task<string> GetCalendarContentAsync(
        string calendarUrl,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(calendarUrl);

        using var response = await httpClient.GetAsync(calendarUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }


    /// <summary>
    /// Retrieves and parses deadlines from the user's HSE calendar.
    /// </summary>
    /// <param name="userId">The calendar owner's user ID.</param>
    /// <param name="authToken">The authentication token for the calendar export.</param>
    /// <param name="cancellationToken">A token to cancel the calendar request or response read.</param>
    /// <returns>The parsed deadlines, with course names populated when available.</returns>
    /// <exception cref="ArgumentException">The parsed calendar is null.</exception>
    public async Task<List<Deadline>> GetDeadlinesAsync(
        string userId,
        string authToken,
        CancellationToken cancellationToken = default)
    {
        var calendarUrl =
            $"https://edu.hse.ru/calendar/export_execute.php?userid={userId}&authtoken={authToken}&preset_what=all&preset_time=recentupcoming";
        var calendar = Calendar.Load(await GetCalendarContentAsync(calendarUrl, cancellationToken));
        if (calendar == null)
        {
            throw new ArgumentException("Calendar content is null.", nameof(calendarUrl));
        }

        var deadlines = calendar.Events.Select(Deadline.FromEvent).ToList();
        foreach (var deadline in deadlines)
        {
            if (courseNamesProvider.CourseNames.TryGetValue(deadline.CourseId, out var courseName))
                deadline.CourseName = Regex.Replace(
                    courseName,
                    @"\s*\(\d{4}/\d{4}\s+модули:\s*[^)]*\)\s*$",
                    "",
                    RegexOptions.IgnoreCase).TrimEnd();
            else
                courseNamesProvider.AddCourseIds(deadline.CourseId);

            deadline.Summary = deadline.Summary.Replace(" - надо сдать", "");
        }

        return deadlines;
    }
}