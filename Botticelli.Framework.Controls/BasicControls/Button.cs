﻿namespace Botticelli.Framework.Controls.BasicControls;

public class Button : IControl
{
    public string? Content { get; set; }
    public Dictionary<string, string>? Params { get; set; } = new();

    public Dictionary<string, Dictionary<string, object>>? MessengerSpecificParams { get; set; } = new();
}