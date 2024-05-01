using CodeExecutor.Dispatcher.Contracts;

namespace BaseCSharpExecutor.Api;

public interface ICodeExecutionDispatcherClient
{
    public Task SetResultAsync(CodeExecutionResult codeExecutionResult, string validationTag, CancellationToken cancellationToken = default);
}