using CodeExecutor.Dispatcher.Contracts;


namespace BaseCSharpExecutor.Api;

/// <summary>
/// Api client for CodeExecutor.Dispatcher service.
/// </summary>
public interface ICodeExecutionDispatcherClient
{
    public Task SetResultAsync(CodeExecutionResult codeExecutionResult, string validationTag, CancellationToken cancellationToken = default);
}