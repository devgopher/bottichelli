using Botticelli.Server.Back.Services.Auth.Models;

namespace Botticelli.Server.Back.Services.Auth;

/// <summary>
///     Code generator
/// </summary>
public interface IConfirmationCodeGenerator
{
    /// <summary>
    ///     Generates a code
    /// </summary>
    /// <param name="size"></param>
    /// <param name="lifetime"></param>
    /// <returns></returns>
    public ConfirmationCode GenerateCode(int size = 4,  TimeSpan lifetime = default);
}