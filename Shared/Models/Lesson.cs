using System.Linq.Expressions;

namespace Shared.Models;

public class Lesson
{
    public int Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int? Subgroup { get; set; }
    public bool IsOptional { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Teacher { get; set; }
    public string? Link { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public static Lesson Empty => new();
    
    public static Expression<Func<Lesson, bool>> IsDuplicateOf(Lesson target)
    {
        return x =>
            x.GroupName == target.GroupName &&
            x.Subgroup == target.Subgroup &&
            x.IsOptional == target.IsOptional &&
            x.Name == target.Name &&
            x.Location == target.Location &&
            x.Teacher == target.Teacher &&
            x.Link == target.Link &&
            x.StartTime == target.StartTime &&
            x.EndTime == target.EndTime;
    }
}