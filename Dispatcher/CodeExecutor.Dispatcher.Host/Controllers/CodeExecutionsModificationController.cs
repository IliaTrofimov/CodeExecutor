using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Host.Services.Interfaces;


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
    public async Task<ActionResult> SetResult([FromBody] CodeExecutionResult executionResult, [FromHeader] string validationTag)
    {
        if (string.IsNullOrWhiteSpace(validationTag))
            throw new BadRequestException("ValidationTag header cannot be empty");
   
        await dispatcher.SetExecutionResultsAsync(executionResult, validationTag);
        return Ok();
    }
}


