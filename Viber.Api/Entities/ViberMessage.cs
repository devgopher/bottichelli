using System.Text.Json.Serialization;

namespace Viber.Api.Entities
{
    public class ViberMessage
    {
        [JsonPropertyName("type")] public string? Type { get; set; }

        [JsonPropertyName("text")] public string? Text { get; set; }

        [JsonPropertyName("media")] public string? Media { get; set; }

        [JsonPropertyName("location")] public Location? Location { get; set; }

        [JsonPropertyName("tracking_data")] public string? TrackingData { get; set; }
    }
}