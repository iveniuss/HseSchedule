using System.Text.Json.Serialization;

namespace ScheduleUpdater.Models;

public class YdItem
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("file")]
    public string? File { get; set; }
}