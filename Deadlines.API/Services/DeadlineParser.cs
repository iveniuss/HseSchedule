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

    public async Task<List<Deadline>> GetDeadlinesAsync(
        string calendarUrl,
        CancellationToken cancellationToken = default)
    {
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

            deadline.Summary = deadline.Summary.Replace(" - надо сдать", "");
        }

        return deadlines;
    }
    
    
}
