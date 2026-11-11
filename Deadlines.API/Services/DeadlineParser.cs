using Deadlines.API.Models;
using Ical.Net;

namespace Deadlines.API.Services;

public class DeadlineParser(HttpClient httpClient)
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
        
        return calendar.Events.Select(Deadline.FromEvent).ToList();
    }
}