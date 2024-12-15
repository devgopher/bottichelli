using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.Messages.API.Markups;

public class VkKeyboardMarkup
{
    [JsonPropertyName("one_time")] public bool OneTime { get; set; }

    [JsonPropertyName("buttons")] public IEnumerable<IEnumerable<VkItem>> Buttons { get; set; }
}