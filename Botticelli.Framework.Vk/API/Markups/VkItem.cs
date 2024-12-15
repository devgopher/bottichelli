using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Markups;

public class VkItem
{
    [JsonPropertyName("action")] public Action Action { get; set; }

    [JsonPropertyName("color")] public string Color { get; set; }
}