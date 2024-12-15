using Botticelli.Framework.Options;

namespace Botticelli.Framework.Telegram.Options;

/// <inheritdoc />
public class TelegramBotSettings : BotSettings
{
    public static string Section => "TelegramBot";
    
    /// <summary>
    /// Timeout in ms
    /// </summary>
    public int Timeout { get; set; } = 60000;

    /// <summary>
    /// Use throttling or not?
    /// </summary>
    public bool? UseThrottling { get; set; } = true;
    
    /// <summary>
    /// Should we use test environment
    /// </summary>
    public bool? UseTestEnvironment { get; set; } = false;

    /// <summary>
    /// Base url for Telegram API
    /// </summary>
    public string? TelegramBaseUrl { get; set; } = "https://api.telegram.org";
}