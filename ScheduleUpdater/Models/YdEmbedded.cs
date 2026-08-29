using System.Text.Json.Serialization;

namespace ScheduleUpdater.Models;

public class YdEmbedded
{
    [JsonPropertyName("items")]
    public List<YdItem> Items { get; set; } = new();
}