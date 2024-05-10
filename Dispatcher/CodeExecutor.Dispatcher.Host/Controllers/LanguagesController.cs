using CodeExecutor.Dispatcher.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CodeExecutor.Dispatcher.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        List<Language> languages = await languagesService.GetListAsync(skip, take);
        return Ok(languages);
    }

    /// <summary>Get list of all available programming languages' versions.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ListVersions([FromQuery] string languageName)
    {
        List<Language> languages = await languagesService.GetVersionsListAsync(languageName);
        return Ok(languages);
    }
}