﻿using System.Text.Json.Serialization;

namespace Botticelli.Framework.Vk.API.Responses;

public class NewPts
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}