using Botticelli.Server.Data.Entities.Auth;
using Botticelli.Server.Data.Exceptions;
using Botticelli.Server.Models.Responses;

namespace Botticelli.Server.Back.Services.Auth;

public interface IAdminAuthService
{
    /// <summary>
    ///     Do we have any users?
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DataException"></exception>
    Task<bool> HasUsersAsync();

    /// <summary>
    ///     Registers an admin user
    /// </summary>
    /// <param name="userRegister"></param>
    /// <returns></returns>
    /// <exception cref="DataException"></exception>
    Task RegisterAsync(UserAddRequest userRegister);

    /// <summary>
    ///     Regenerates password for user
    /// </summary>
    /// <param name="userRegister"></param>
    /// <returns></returns>
    /// <exception cref="DataException"></exception>
    Task RegeneratePassword(UserAddRequest userRegister);


    /// <summary>
    ///     Generates auth token
    /// </summary>
    /// <param name="login"></param>
    /// <returns></returns>
    GetTokenResponse GenerateToken(UserLoginRequest login);

    /// <summary>
    ///     Checks auth token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    bool CheckToken(string token);

    /// <summary>
    ///     Checks access
    /// </summary>
    /// <param name="login"></param>
    /// <param name="checkEmailConfirmed"></param>
    /// <returns></returns>
    public (bool result, string err) CheckAccess(UserLoginRequest login, bool checkEmailConfirmed);

    string GetCurrentUserId();
}