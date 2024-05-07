namespace CodeExecutor.Dispatcher.Services.Interfaces;

/// <summary>
/// Message queue for dispatching code executions. 
/// </summary>
public interface ICodeExecutionMessaging
{
    public Task SendStartMessageAsync(CodeExecutionExpanded codeExecution, string executionKey,
        ExecutionPriority priority = ExecutionPriority.Normal);

}