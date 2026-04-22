using Shared.Models;

namespace ScheduleAPI.Models;

public class LessonDto
{
    public string GroupName { get; set; } = string.Empty;
    public int? Subgroup { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Teacher { get; set; }
    public string? Link { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public static LessonDto FromLesson(Lesson lesson) => new LessonDto()
    {
        GroupName = lesson.GroupName,
        Subgroup = lesson.Subgroup,
        Name = lesson.Name,
        Location = lesson.Location,
        Teacher = lesson.Teacher,
        Link = lesson.Link,
        StartTime = lesson.StartTime,
        EndTime = lesson.EndTime
    };
}