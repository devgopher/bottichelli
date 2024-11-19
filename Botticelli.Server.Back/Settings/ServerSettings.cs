using FluentEmail.MailKitSmtp;

namespace Botticelli.Server.Back.Settings;

public class ServerSettings
{
    /// <summary>
    ///     Token lifetime minutes
    /// </summary>
    public int TokenLifetimeMin { get; set; }

    public required SmtpClientOptions SmtpClientOptions { get; set; }

    public required string ServerEmail { get; set; }

    public int HttpsPort { get; set; } 
    public required string ServerUrl { get; set; }
    public string? AnalyticsUrl { get; set; }
    public required string SecureStorageConnection { get; set; }
    public bool UseSsl { get; set; }
}