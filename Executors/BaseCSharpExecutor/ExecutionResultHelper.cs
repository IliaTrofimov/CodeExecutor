using CodeExecutor.Dispatcher.Contracts;

namespace BaseCSharpExecutor;

public class ExecutionResultHelper
{
    public Guid Guid { get; }

    public ExecutionResultHelper(Guid guid)
    {
        Guid = guid;
    }

    public CodeExecutionResult GetError(string? comment, string? data = null) => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = true,
        Data = data,
        Status = CodeExecutionStatus.Error,
        Comment = comment
    };
    
    public CodeExecutionResult GetError(int errorsCount, string? data = null) => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = true,
        Data = data,
        Status = CodeExecutionStatus.Error,
        Comment = $"Execution finished with {errorsCount} errors",
    };

    public CodeExecutionResult GetSuccess(string? data = null) => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = false,
        Data = data,
        Status = CodeExecutionStatus.Finished,
        Comment = "Execution finished successfully"
    };
    
    public CodeExecutionResult GetStarted() => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = false,
        Comment = "Execution has started",
        Status = CodeExecutionStatus.Started
    };
}