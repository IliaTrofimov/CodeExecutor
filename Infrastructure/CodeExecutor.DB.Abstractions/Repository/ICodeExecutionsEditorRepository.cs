using CodeExecutor.DB.Abstractions.Models;

namespace CodeExecutor.DB.Abstractions.Repository;


/// <summary>
/// Object for editing code executions.
/// </summary>
public interface ICodeExecutionsEditorRepository : IRepository
{
    /// <summary>Create new code execution.</summary>
    public Task<CodeExecution> Create(long userId, long languageId, string sourceCode,
        CancellationToken cancellationToken = default);
    
    /// <summary>Add new results to code execution.</summary>
    public Task<CodeExecution?> SetResult(Guid guid, string? data, string? comment = null,
        bool? isStarted = null,  bool? isFinished = null, bool? isError = null,
        CancellationToken cancellationToken = default);

    /// <summary>Delete code execution.</summary>
    public Task Delete(Guid guid, CancellationToken cancellationToken = default);
}