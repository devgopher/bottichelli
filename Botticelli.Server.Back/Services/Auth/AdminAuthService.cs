using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Botticelli.Server.Back.Settings;
using Botticelli.Server.Data;
using Botticelli.Server.Data.Entities.Auth;
using Botticelli.Server.Data.Exceptions;
using Botticelli.Server.Models.Responses;
using Botticelli.Shared.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Botticelli.Server.Back.Services.Auth;

/// <summary>
///     Authentication service
/// </summary>
public class AdminAuthService : IAdminAuthService
{
    private readonly IConfiguration _config;
    private readonly ServerDataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AdminAuthService> _logger;
    private readonly IOptionsMonitor<ServerSettings> _settings;

    public AdminAuthService(IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        ServerDataContext context,
        ILogger<AdminAuthService> logger,
        IOptionsMonitor<ServerSettings> settings)
    {
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _logger = logger;
        _settings = settings;
    }

    /// <summary>
    ///     Do we have any users?
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DataException"></exception>
    public async Task<bool> HasUsersAsync()
        => await _context
            .ApplicationUsers
            .AnyAsync();

    /// <inheritdoc />
    public async Task RegisterAsync(UserAddRequest userRegister)
    {
        try
        {
            _logger.LogInformation($"{nameof(RegisterAsync)}({userRegister.UserName}) started...");

            if (_context.ApplicationUsers.AsQueryable()
                .Any(u => u.NormalizedEmail == GetNormalized(userRegister.Email)))
                throw new DataException($"User with email {userRegister.Email} already exists!");

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = userRegister.Email,
                NormalizedEmail = GetNormalized(userRegister.Email),
                UserName = userRegister.Email,
                NormalizedUserName = GetNormalized(userRegister.Email),
                PasswordHash = HashUtils.GetHash(userRegister.Password, _config["Authorization:Salt"])
            };

            // Temporary - because now we assume, that we've only a single role - "admin"! 
            var appRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = _context.ApplicationRoles.FirstOrDefault()?.Id ?? "-1"
            };

            await _context.ApplicationUsers.AddAsync(user);
            await _context.ApplicationUserRoles.AddAsync(appRole);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"{nameof(RegisterAsync)}({userRegister.UserName}) finished...");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(RegisterAsync)}({userRegister.UserName}) error: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task RegeneratePassword(UserAddRequest userRegister)
    {
        try
        {
            _logger.LogInformation($"{nameof(RegeneratePassword)}({userRegister.UserName}) started...");

            if (_context.ApplicationUsers.AsQueryable()
                .Any(u => u.NormalizedEmail == GetNormalized(userRegister.Email)))
                throw new DataException($"User with email {userRegister.Email} already exists!");

            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = userRegister.Email,
                NormalizedEmail = GetNormalized(userRegister.Email),
                UserName = userRegister.Email,
                NormalizedUserName = GetNormalized(userRegister.Email),
                PasswordHash = HashUtils.GetHash(userRegister.Password, _config["Authorization:Salt"])
            };

            // Temporary - because now we assume, that we've only a single role - "admin"! 
            var appRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = _context.ApplicationRoles.FirstOrDefault()?.Id ?? "-1"
            };


            await _context.SaveChangesAsync();

            _logger.LogInformation($"{nameof(RegeneratePassword)}({userRegister.UserName}) finished...");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(RegeneratePassword)}({userRegister.UserName}) error: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public GetTokenResponse GenerateToken(UserLoginRequest login)
    {
        try
        {
            _logger.LogInformation($"{nameof(GenerateToken)}({login.Email}) started...");

            if (!CheckAccess(login, false).result)
            {
                _logger.LogInformation($"{nameof(GenerateToken)}({login.Email}) access denied...");

                return new GetTokenResponse
                {
                    IsSuccess = false,
                    Token = null
                };
            }

            var user = _context.ApplicationUsers
                .AsQueryable()
                .FirstOrDefault(u => u.NormalizedEmail == GetNormalized(login.Email));

            if (user == default)
                return new GetTokenResponse
                {
                    IsSuccess = false,
                    Token = null
                };

            var userRole = _context.ApplicationUserRoles
                .AsQueryable()
                .FirstOrDefault(ur => ur.UserId == user.Id);

            var roleName = string.Empty;

            if (userRole != null)
            {
                var role = _context.ApplicationRoles
                    .AsQueryable()
                    .FirstOrDefault(r => r.Id == userRole.RoleId);

                roleName = role?.Name;
            }

            var claims = new[]
            {
                new Claim("applicationUserId", user.Id),
                new Claim("applicationUserName", user.UserName),
                new Claim("role", roleName ?? "no_role")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authorization:Key"]));
            var signCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Authorization:Issuer"],
                _config["Authorization:Audience"],
                claims,
                expires: DateTime.Now.AddHours(24), // NOTE!!! Temporary!
                //.AddMinutes(_settings.CurrentValue.TokenLifetimeMin),
                signingCredentials: signCreds);

            return new GetTokenResponse
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(GenerateToken)}({login.Email}) error {ex.Message}!");
        }

        return default;
    }


    /// <inheritdoc />
    public bool CheckToken(string token)
    {
        try
        {
            _logger.LogInformation($"{nameof(CheckToken)}() started...");

            var sign = _config["Authorization:Key"];
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sign)),
                    ValidIssuer = _config["Authorization:Issuer"],
                    ValidateAudience = false
                },
                out var validatedToken);


            _logger.LogInformation($"{nameof(CheckToken)}() validate token: {validatedToken != default}");

            return validatedToken != default;
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(CheckToken)}() error: {ex.Message}");

            return false;
        }
    }

    /// <inheritdoc />
    public (bool result, string err) CheckAccess(UserLoginRequest login, bool checkEmailConfirmed)
    {
        var hashedPassword = HashUtils.GetHash(login.Password, _config["Authorization:Salt"]);
        var normalizedEmail = GetNormalized(login.Email);
        var user = _context.ApplicationUsers.FirstOrDefault(u => u.NormalizedEmail == normalizedEmail &&
                                                                 u.PasswordHash == hashedPassword);
        if (user == default)
            return (false, "user not found");
        if (checkEmailConfirmed && !user.EmailConfirmed)
            return (false, $"email {user.Email} not confirmed!");

        return (true, string.Empty);
    }

    public string? GetCurrentUserId()
        => _httpContextAccessor.HttpContext
            .User
            .Claims
            .FirstOrDefault(c => c.Type == "applicationUserId")
            ?.Value;

    private static string GetNormalized(string input)
        => input?.ToUpper();
}