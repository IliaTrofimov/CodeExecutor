using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using CodeExecutor.Dispatcher.Services.Interfaces;


namespace CodeExecutor.Dispatcher.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
public sealed class LanguagesController : ControllerBase
{
    private readonly ILogger<LanguagesController> logger;
    private readonly IProgrammingLanguagesService languagesService;
    
    
    public LanguagesController(ILogger<LanguagesController> logger, IProgrammingLanguagesService languagesService)
    {
        this.logger = logger;
        this.languagesService = languagesService;
    }
    
    
    /// <summary>Get list of all available programming languages.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> List([FromQuery] int? skip = null, [FromQuery] int? take = null)
    {
        var languages = await languagesService.GetListAsync(skip, take);
        return Ok(languages);
    }
    
    /// <summary>Get list of all available programming languages' versions.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListVersions([FromQuery] string languageName)
    {
        var languages = await languagesService.GetVersionsListAsync(languageName);
        return Ok(languages);
    }
}