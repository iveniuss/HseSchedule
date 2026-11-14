using System.Text.Json;

namespace Deadlines.API.Services;

public class CourseNamesProvider
{
    private readonly string _dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");
    private string CourseNamesPath => Path.Combine(_dataDirectory, "courseNames.json");
    private string ToAddPath => Path.Combine(_dataDirectory, "ToAdd.json");
    
    private Dictionary<string, string> _courseNames;
    private List<string> _toAdd;
    public IReadOnlyDictionary<string, string> CourseNames => _courseNames;
    public IReadOnlyList<string> ToAdd => _toAdd;

    public CourseNamesProvider()
    {
        _courseNames = new Dictionary<string, string>();
        _toAdd = new List<string>();
        LoadCourseNames();
    }
    
    public void LoadCourseNames()
    {
        Directory.CreateDirectory(_dataDirectory);
        CreateFileIfMissing(CourseNamesPath, "{}");
        CreateFileIfMissing(ToAddPath, "[]");

        var json = File.ReadAllText(CourseNamesPath);
        _courseNames = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                      ?? new Dictionary<string, string>();
        
        json = File.ReadAllText(ToAddPath);
        _toAdd = JsonSerializer.Deserialize<List<string>>(json)
                ?? new List<string>();
    }

    public void AddCourseIds(string courseId)
    {
        _toAdd.Add(courseId);
        
        var json = JsonSerializer.Serialize(ToAdd);
        File.WriteAllText(ToAddPath, json);
    }

    private static void CreateFileIfMissing(string path, string initialContent)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, initialContent);
        }
    }
}
