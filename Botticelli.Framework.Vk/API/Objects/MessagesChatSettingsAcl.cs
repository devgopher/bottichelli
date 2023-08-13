﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Objects;

public class MessagesChatSettingsAcl
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("properties")]
    public Properties Properties { get; set; }

    [JsonPropertyName("additionalProperties")]
    public bool AdditionalProperties { get; set; }
}