namespace Botticelli.Server.Back.Services.Auth.Models;

public class ConfirmationCode
{
    private DateTime GeneratedAt { get; } = DateTime.UtcNow;
    public required string Code { get; init; }
    public required TimeSpan Lifetime { get; init; }

    public bool IsExpired => DateTime.UtcNow > GeneratedAt + Lifetime;
}