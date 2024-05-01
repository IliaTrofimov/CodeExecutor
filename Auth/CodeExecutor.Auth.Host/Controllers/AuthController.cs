using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeExecutor.Auth.Contracts;
using CodeExecutor.Auth.Host.Services;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace CodeExecutor.Auth.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    
    
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    
    /// <summary>Get current user information.</summary>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<UserInfo> CurrentUser([FromHeader] string? anonymousUserToken = null)
    {
        if (!HttpContext.TryParseUser(out var user))
            throw new UnauthorizedException();

        var userInfo = new UserInfo
        {
            UserId = user.Id,
            Username = user.Username,
            IsSuperUser = user.IsSuperUser
        };
        return Ok(userInfo);
    }
    
    /// <summary>Login with given username and password.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return Ok(response);
    }
    
    /// <summary>Create new user.</summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Create([FromBody] CreateUserRequest request)
    {
        var response = await authService.CreteUserAsync(request);
        return Ok(response);
    }
}