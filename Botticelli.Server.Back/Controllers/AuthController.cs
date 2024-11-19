using Botticelli.Server.Back.Services.Auth;
using Botticelli.Server.Data.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Botticelli.Server.Controllers;

/// <summary>
///     Login controller for login/logoff/registration and access checking functions
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("/v1/auth")]
public class AuthController
{
    private readonly IAdminAuthService _adminAuthService;

    public AuthController(IAdminAuthService adminAuthService)
    {
        _adminAuthService = adminAuthService;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public IActionResult GetToken(UserLoginRequest request)
        => new OkObjectResult(_adminAuthService.GenerateToken(request));
}