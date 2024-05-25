using System.Diagnostics;
using CodeExecutor.Common.Security;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Telemetry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;


namespace CodeExecutor.Dispatcher.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        using var activity = TraceRoot.Start("Execution preprocessing");
        
        var response = await dispatcher.StartCodeExecutionAsync(request, appUser.Id);
        var url = HttpContext.Request
            .GetEncodedUrl()
            .Replace("execute", $"result/{response.Guid}", StringComparison.OrdinalIgnoreCase);

        if (activity is not null)
        {
            activity.AddTag("execution.Id", response.Guid);
            activity.AddTag("execution.LanguageId", request.LanguageId);
            response.TraceId = activity.RootId ?? activity.Id;
        }
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
        if (result is null) return NoContent();

        Activity.Current?.AddTag("execution.Id", guid);
        return Ok(result);
    }

    /// <summary>Get code execution source code.</summary>
    [HttpGet("{guid}")]
    [Authorize]
    public async Task<ActionResult<SourceCode>> SourceCode(Guid guid)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();

        var sourceCode = await explorer.GetSourceCodeAsync(guid, appUser.Id);
        if (sourceCode is null) return NoContent();

        Activity.Current?.AddTag("execution.Id", guid);
        return Ok(sourceCode);
    }

    /// <summary>Get code executions for given user.</summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ICollection<CodeExecution>>> List([FromQuery] int? skip = null,
                                                                     [FromQuery] int? take = null)
    {
        if (!HttpContext.TryParseUser(out var appUser))
            throw new UnauthorizedException();

        List<CodeExecution> results = await explorer.GetExecutionsListAsync(appUser.Id, skip, take);
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