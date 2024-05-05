using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Common.Security;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace CodeExecutor.Dispatcher.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
public sealed class CodeExecutionsController : ControllerBase
{
    private readonly ILogger<CodeExecutionsController> logger;
    private readonly ICodeExecutionDispatcher dispatcher;
    private readonly ICodeExecutionExplorer explorer;


    public CodeExecutionsController(ILogger<CodeExecutionsController> logger, 
        ICodeExecutionExplorer explorer, 
        ICodeExecutionDispatcher dispatcher)
    {
        this.logger = logger;
        this.explorer = explorer;
        this.dispatcher = dispatcher;
    }

    /// <summary>Start new code execution.</summary>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CodeExecutionStartResponse>> Execute([FromBody] CodeExecutionRequest request)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();

        var response = await dispatcher.StartCodeExecutionAsync(request, appUser.Id);
        
        var url = HttpContext.Request
            .GetEncodedUrl()
            .Replace("execute", $"result/{response.Guid}", StringComparison.OrdinalIgnoreCase);
        
        return Created(url, response);
    }
    
    /// <summary>Get code execution results.</summary>
    [HttpGet("{guid}")]
    [Authorize]
    public async Task<ActionResult<CodeExecutionExpanded>> Result(Guid guid)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();
        
        var result = await explorer.GetExecutionResultAsync(guid, appUser.Id);
        return result is not null
            ? Ok(result)
            : NoContent();
    }
    
    /// <summary>Get code execution source code.</summary>
    [HttpGet("{guid}")]
    [Authorize]
    public async Task<ActionResult<SourceCode>> SourceCode(Guid guid)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();
        
        var sourceCode = await explorer.GetSourceCodeAsync(guid, appUser.Id);
        return sourceCode is not null
            ? Ok(sourceCode)
            : NoContent();
    }

    /// <summary>Get code executions for given user.</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ICollection<CodeExecution>>> List(
        [FromQuery] int? skip = null,
        [FromQuery] int? take = null)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();
        
        var results = await explorer.GetExecutionsListAsync(appUser.Id, skip, take);
        return Ok(results);
    }
    
    /// <summary>Delete code execution.</summary>
    [HttpDelete("{guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid guid)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();
        
        await dispatcher.DeleteCodeExecutionAsync(guid, appUser.Id);
        return Ok();
    }
}


