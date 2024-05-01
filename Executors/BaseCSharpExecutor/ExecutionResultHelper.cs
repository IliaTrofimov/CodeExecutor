using CodeExecutor.Dispatcher.Contracts;

namespace BaseCSharpExecutor;

public class ExecutionResultHelper
{
    public Guid Guid { get; private set; }

    public ExecutionResultHelper(Guid guid)
    {
        Guid = guid;
    }

    public CodeExecutionResult GetError(string? comment, string? data = null) => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = true,
        Comment = comment,
        Data = data,
        Status = CodeExecutionStatus.Error
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
        Comment = "Execution finished successfully",
        Data = data,
        Status = CodeExecutionStatus.Finished
    };
    
    public CodeExecutionResult GetStarted() => new CodeExecutionResult
    {
        Guid = Guid,
        IsError = false,
        Comment = "Execution has started",
        Status = CodeExecutionStatus.Started
    };
}