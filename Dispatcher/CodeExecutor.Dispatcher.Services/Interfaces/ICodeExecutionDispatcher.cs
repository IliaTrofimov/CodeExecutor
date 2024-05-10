namespace CodeExecutor.Dispatcher.Services.Interfaces;

/// <summary>Create new code executions.</summary>
public interface ICodeExecutionDispatcher
{
    /// <summary>Start new code execution.</summary>
    public Task<CodeExecutionStartResponse> StartCodeExecutionAsync(CodeExecutionRequest request, long userId);

    /// <summary>Delete code execution.</summary>
    public Task DeleteCodeExecutionAsync(Guid executionGuid, long userId);

    /// <summary>Append code execution results.</summary>
    public Task SetExecutionResultsAsync(CodeExecutionResult codeExecution, string validationTag);
}