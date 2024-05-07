namespace CodeExecutor.Dispatcher.Services.Interfaces;

/// <summary>
/// Manage current code executions.
/// </summary>
public interface ICodeExecutionExplorer
{
    /// <summary>Load code execution with given guid.</summary>
    public Task<CodeExecutionExpanded?> GetExecutionResultAsync(Guid executionGuid, long userId);
    
    /// <summary>Load source code for executions with given guid.</summary>
    public Task<SourceCode?> GetSourceCodeAsync(Guid executionGuid, long userId);
    
    /// <summary>Load code executions for given user.</summary>
    public Task<List<CodeExecution>> GetExecutionsListAsync(long userId, int? skip = null, int? take = null, IEnumerable<Guid>? guids = null);
}