using BaseCSharpExecutor.Api;
using CodeExecutor.Dispatcher.Contracts;
using Microsoft.Extensions.Logging;


namespace CodeExecutor.UnitTests.Mocks.Services;

public class CodeExecutionDispatcherApiMock : ICodeExecutionDispatcherClient
{
    protected readonly ILogger? Logger;
    
    public Action<CodeExecutionResult>? OnSetResult { get; set; }
    public Action<CodeExecutionResult>? OnSetError { get; set; } 
    public Action<CodeExecutionResult>? OnStarted { get; set; }
    public Action<CodeExecutionResult>? OnUpdate { get; set; }

    
    public CodeExecutionDispatcherApiMock(ILogger? logger = null)
    {
        Logger = logger;
        OnSetError = AssertNotError;
    }
    
    
    public Task SetResultAsync(CodeExecutionResult codeExecutionResult, string validationTag,
        CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(SetResultAsync)}");
        
        Assert.False(string.IsNullOrWhiteSpace(validationTag), 
            $"CodeExecution {nameof(validationTag)} cannot be empty at {nameof(SetResultAsync)}()");
        Assert.NotEqual(new Guid(), codeExecutionResult.Guid);

        OnCallback(codeExecutionResult);
        return Task.CompletedTask;
    }


    private void AssertNotError(CodeExecutionResult result)
    {
        Assert.Fail($"CodeExecution finished with error:\n{result.Comment}\n\n{result.Data}");
    }
    

    private void OnCallback(CodeExecutionResult codeExecutionResult)
    {
        var handler = codeExecutionResult.Status switch
        {
            CodeExecutionStatus.Started => OnStarted,
            CodeExecutionStatus.Error => OnSetError,
            CodeExecutionStatus.Finished => OnSetResult,
            null => OnUpdate,
            _ => null
        };
        if (handler is null) return;
        
        Logger?.LogDebug($"MOCK Callback {codeExecutionResult.Status}");
        handler.Invoke(codeExecutionResult);
    }
}