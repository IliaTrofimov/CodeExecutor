using System.Diagnostics;
using CodeExecutor.Dispatcher.Contracts;


namespace BaseCSharpExecutor;

public class ExecutionResultHelper
{
    public Guid Guid { get; }

    public ExecutionResultHelper(Guid guid) { Guid = guid; }

    public CodeExecutionResult GetError(string? comment, string? data = null) =>
        new()
        {
            Guid = Guid,
            IsError = true,
            Data = data,
            Status = CodeExecutionStatus.Error,
            Comment = comment,
            TraceId = Activity.Current?.Id
        };

    public CodeExecutionResult GetError(int errorsCount, string? data = null) =>
        new()
        {
            Guid = Guid,
            IsError = true,
            Data = data,
            Status = CodeExecutionStatus.Error,
            Comment = $"Execution finished with {errorsCount} errors",
            TraceId = Activity.Current?.Id
        };

    public CodeExecutionResult GetSuccess(string? data = null) =>
        new()
        {
            Guid = Guid,
            IsError = false,
            Data = data,
            Status = CodeExecutionStatus.Finished,
            Comment = "Execution finished successfully",
            TraceId = Activity.Current?.Id
        };

    public CodeExecutionResult GetStarted() =>
        new()
        {
            Guid = Guid,
            IsError = false,
            Comment = "Execution has started",
            Status = CodeExecutionStatus.Started,
            TraceId = Activity.Current?.Id
        };
}