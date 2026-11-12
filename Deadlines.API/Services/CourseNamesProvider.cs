using System.Text.Json;

namespace Deadlines.API.Services;

public class CourseNamesProvider
{
    public Dictionary<string, string> CourseNames { get; private set; }

    public CourseNamesProvider()
    {
        CourseNames = new Dictionary<string, string>();
        LoadCourseNames();
    }
    
    public void LoadCourseNames()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory,"Data", "courseNames.json");
        var json = File.ReadAllText(filePath);
        CourseNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                      ?? new Dictionary<string, string>();
    }
}
