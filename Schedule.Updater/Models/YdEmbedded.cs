using System.Text.Json.Serialization;

namespace Schedule.Updater.Models;

public class YdEmbedded
{
    [JsonPropertyName("items")]
    public List<YdItem> Items { get; set; } = new();
}