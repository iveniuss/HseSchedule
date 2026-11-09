using System.Text.Json.Serialization;

namespace Schedule.Updater.Models;

public class YdResponse
{
    [JsonPropertyName("_embedded")]
    public YdEmbedded? Embedded { get; set; }
}