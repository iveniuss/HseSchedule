using Ical.Net.CalendarComponents;

namespace Deadlines.API.Models;

public class Deadline
{
    public required string Id { get; set; }
    public required string CourseId { get; set; }
    public string? CourseName { get; set; }
    public required string Summary { get; set; }
    public required string Description { get; set; }
    public required DateTime DeadlineDate { get; set; }

    public static Deadline FromEvent(CalendarEvent e)
    {
        return new Deadline
        {
            Id = e.Uid?.Split("@")[0] ?? "NO_ID",
            CourseId = e.Categories.First(),
            Summary = e.Summary ?? "NO_SUMMARY",
            Description = e.Description ?? "NO_DESCRIPTION",
            DeadlineDate = e.Start?.AsUtc ?? DateTime.MinValue
        };
    }
}