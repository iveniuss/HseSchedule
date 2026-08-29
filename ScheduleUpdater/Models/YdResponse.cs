using System.Text.Json.Serialization;

namespace ScheduleUpdater.Models;

public class YdResponse
{
    [JsonPropertyName("_embedded")]
    public YdEmbedded? Embedded { get; set; }
}