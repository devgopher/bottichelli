using System.Text.Json.Serialization;

namespace Viber.Api.Entities
{
    public class Sender
    {
        [JsonPropertyName("name")] public string? Name { get; set; }

        [JsonPropertyName("avatar")] public string? Avatar { get; set; }
    }
}