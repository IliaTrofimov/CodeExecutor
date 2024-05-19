using System.Diagnostics;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Telemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CodeExecutor.Dispatcher.Host.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class CodeExecutionsModificationController : ControllerBase
{
    private readonly ILogger<CodeExecutionsController> logger;
    private readonly ICodeExecutionDispatcher dispatcher;


    public CodeExecutionsModificationController(ILogger<CodeExecutionsController> logger,
                                                ICodeExecutionDispatcher dispatcher)
    {
        this.logger = logger;
        this.dispatcher = dispatcher;
    }


    /// <summary>Set code execution result.</summary>
    [HttpPatch]
    [AllowAnonymous]
    public async Task<ActionResult> SetResult([FromBody] CodeExecutionResult executionResult,
                                              [FromHeader] string validationTag)
    {
        if (string.IsNullOrWhiteSpace(validationTag))
            throw new BadRequestException("ValidationTag header cannot be empty");

        using var activity = TelemetryProvider.StartNew("Execution modification");
        activity?.AddTag("execution.Id", executionResult.Guid);
        activity?.AddTag("execution.Status", executionResult.Status);
        activity?.AddTag("execution.IsError", executionResult.IsError);

        await dispatcher.SetExecutionResultsAsync(executionResult, validationTag);
        return Ok();
    }
}