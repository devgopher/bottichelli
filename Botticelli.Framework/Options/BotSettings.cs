namespace Botticelli.Framework.Options;

/// <summary>
///     Bot general settings
/// </summary>
public abstract class BotSettings : IBotSettings
{
    /// <summary>
    ///     Bot name
    /// </summary>
    public string? Name { get; set; }
    
    public string BotCertificateName { get; set; } = "BotticelliBotsBot";
}