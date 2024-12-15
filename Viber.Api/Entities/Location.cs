using System.Text.Json.Serialization;

namespace Viber.Api.Entities
{
    public class Location
    {
        [JsonPropertyName("lat")] public double Lat { get; set; }

        [JsonPropertyName("lon")] public double Lon { get; set; }
    }
}