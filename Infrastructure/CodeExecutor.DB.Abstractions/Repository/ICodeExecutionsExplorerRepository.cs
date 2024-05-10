using CodeExecutor.DB.Abstractions.Models;


namespace CodeExecutor.DB.Abstractions.Repository;

/// <summary>Interface for viewing code executions.</summary>
public interface ICodeExecutionsExplorerRepository : IReadonlyRepository<CodeExecution>
{
    /// <summary>Get base code execution data with its source code.</summary>
    public Task<CodeExecution?> GetSourceCodeAsync(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>Get base code execution data with its results.</summary>
    public Task<CodeExecution?> GetResultAsync(Guid guid, CancellationToken cancellationToken = default);

    /// <summary>Check if given validationTag matches CodeExecution.SecretKey.</summary>
    public Task<bool> CheckSecretKeyAsync(Guid guid, string validationTag,
                                          CancellationToken cancellationToken = default);
}