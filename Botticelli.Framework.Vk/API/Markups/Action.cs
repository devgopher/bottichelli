using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Markups;

public class Action
{
    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("app_id")] public int AppId { get; set; }

    [JsonPropertyName("owner_id")] public int OwnerId { get; set; }

    [JsonPropertyName("hash")] public string Hash { get; set; }

    [JsonPropertyName("payload")] public string Payload { get; set; }

    [JsonPropertyName("label")] public string Label { get; set; }
}