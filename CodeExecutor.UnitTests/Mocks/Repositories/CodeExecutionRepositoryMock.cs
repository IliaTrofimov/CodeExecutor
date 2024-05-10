using Microsoft.Extensions.Logging;


namespace CodeExecutor.UnitTests.Mocks.Repositories;

public class CodeExecutionRepositoryMock : InMemoryRepository<DBModels.CodeExecution, Guid>,
                                           DBRepo.ICodeExecutionsExplorerRepository,
                                           DBRepo.ICodeExecutionsEditorRepository
{
    public CodeExecutionRepositoryMock(ILogger? logger = null) : base(logger) { }


    public Task<DBModels.CodeExecution?> GetSourceCodeAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(GetSourceCodeAsync)}");
        Data.TryGetValue(guid, out var entity);
        return Task.FromResult(entity);
    }

    public Task<DBModels.CodeExecution?> GetResultAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(GetResultAsync)}");
        Data.TryGetValue(guid, out var entity);
        return Task.FromResult(entity);
    }

    public Task<bool> CheckSecretKeyAsync(Guid guid, string validationTag,
                                          CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(CheckSecretKeyAsync)}");
        return !Data.TryGetValue(guid, out var entity)
            ? Task.FromResult(false)
            : Task.FromResult(entity.SecretKey == validationTag);
    }

    public Task<DBModels.CodeExecution> Create(long userId, long languageId, string sourceCode,
                                               CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(CodeExecutionRepositoryMock)}.{nameof(Create)}");

        var id = NextKey();
        return Create(new DBModels.CodeExecution
        {
            Id = id,
            InitiatorId = userId,
            LanguageId = languageId,
            SourceCode = new DBModels.SourceCode
            {
                Id = id,
                CodeText = sourceCode
            },
            Result = new DBModels.CodeExecutionResult
            {
                Id = id
            }
        });
    }

    public Task<DBModels.CodeExecution?> SetResult(Guid guid, string? data, string? comment = null,
                                                   bool? isStarted = null, bool? isFinished = null,
                                                   bool? isError = null, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(SetResult)}");

        if (!Data.TryGetValue(guid, out var entity))
            return Task.FromResult<DBModels.CodeExecution?>(null);

        entity.Result ??= new DBModels.CodeExecutionResult();
        entity.Result.Data = data;
        entity.UpdatedAt = DateTimeOffset.Now;

        if (isError is not null)
            entity.IsError = isError.Value;

        if (comment is not null)
            entity.Comment = comment;

        if (isFinished == true || isError == true)
            entity.FinishedAt = DateTimeOffset.Now;

        if (isStarted == true)
            entity.StartedAt = DateTimeOffset.Now;

        return Task.FromResult(entity);
    }

    public Task Delete(Guid guid, CancellationToken cancellationToken = default)
    {
        Logger?.LogDebug($"MOCK {nameof(CodeExecutionRepositoryMock)}.{nameof(Delete)}");

        Data.Remove(guid);
        return Task.CompletedTask;
    }


    protected override Guid NextKey() => Guid.NewGuid();
}