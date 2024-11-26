namespace Botticelli.Framework.Options;

/// <summary>
///     Security settings
/// </summary>
public class SecuritySettings
{
    public bool? DisableSecurity { get; set; } = false;
    public string BotCertificateName { get; set; } = "BotticelliBotsBot";
    public string? BotCertificateFingerPrint { get; set; }
    public string? BotCertificatePfxPath { get; set; }
    public string? BotCertificatePassword { get; set; }
    public string? ServerCertificateThumbprint { get; set; }

    public bool? AllowSelfSignedServerCertificate { get; set; }
}