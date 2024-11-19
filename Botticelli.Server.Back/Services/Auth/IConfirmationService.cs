using Microsoft.AspNetCore.Identity;

namespace Botticelli.Server.Back.Services.Auth;

/// <summary>
///     Email confirmation service
/// </summary>
public interface IConfirmationService
{
    /// <summary>
    ///     Sends a confirm code
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task SendConfirmationCode(IdentityUser<string> user, CancellationToken token);

    /// <summary>
    ///     Updates a user
    /// </summary>
    /// <param name="srcToken"></param>
    /// <param name="user"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> ConfirmCodeAsync(string srcToken, IdentityUser<string> user, CancellationToken ct);
}