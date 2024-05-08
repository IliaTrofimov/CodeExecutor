using Microsoft.EntityFrameworkCore;

using CodeExecutor.DB.Abstractions.Models;
using CodeExecutor.DB.Abstractions.Repository;
using CodeExecutor.DB.Exceptions;

namespace CodeExecutor.DB.Repository;

public class CodeExecutionRepository : DefaultEfRepository<CodeExecution>, 
    ICodeExecutionsExplorerRepository,
    ICodeExecutionsEditorRepository
{
    private readonly DbSet<User> users;
    private readonly DbSet<Language> languages;
    
    public CodeExecutionRepository(DataContext context) : base(context)
    {
        users = context.Set<User>();
        languages = context.Set<Language>();
    }
    
    
    public override async Task<CodeExecution?> GetAsync(object key, CancellationToken cancellationToken = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key), "CodeExecution.Id cannot be null");
        if (key is not Guid guid)
            throw new ArgumentException("CodeExecution.Id must be Guid", nameof(key));
        
        var execution = await Query()
            .Include(e => e.Language)
            .Include(e => e.SourceCode)
            .Include(e => e.Result)
            .FirstOrDefaultAsync(e => e.Id == guid, cancellationToken);
       
        return execution;
    }

    public async Task<CodeExecution?> GetSourceCodeAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var execution = await Query()
            .Include(e => e.Language)
            .Include(e => e.SourceCode)
            .FirstOrDefaultAsync(e => e.Id == guid, cancellationToken);
        return execution;
    }
    
    public async Task<CodeExecution?> GetResultAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        var execution = await Query()
            .Include(e => e.Language)
            .Include(e => e.Result)
            .FirstOrDefaultAsync(e => e.Id == guid, cancellationToken);
        return execution;
    }
    
    public async Task<bool> CheckSecretKeyAsync(Guid guid, string validationTag, CancellationToken cancellationToken = default)
    {
        var count = await Query().CountAsync(e => e.Id == guid && e.SecretKey == validationTag, cancellationToken);
        return count == 1;
    }

    
    public async Task<CodeExecution> Create(long userId, long languageId, string sourceCode, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sourceCode))
            throw new ArgumentNullException(nameof(sourceCode), "CodeExecution.SourceCode.CodeText cannot be null");
        if (languageId <= 0)
            throw new ArgumentNullException(nameof(languageId), "CodeExecution.LanguageId cannot be null");
        if (userId < 0)
            throw new ArgumentNullException(nameof(userId), "CodeExecution.UserId cannot be null");

        
        var usersCount = await users.Where(u => u.Id == userId).CountAsync(cancellationToken);
        if (usersCount == 0)
            throw new ConflictException("User not exists");
        
        var languagesCount = await languages.Where(l => l.Id == languageId).CountAsync(cancellationToken);
        if (languagesCount == 0)
            throw new ConflictException("Language not exists");
        
        var execution = dbSet.Add(new CodeExecution
        {
            LanguageId = languageId,
            InitiatorId = userId,
            Result = new CodeExecutionResult(),
            SourceCode = new SourceCode { CodeText = sourceCode }
        });
        return execution.Entity;
    }

    public async  Task<CodeExecution?> SetResult(Guid guid, string? data, string? comment = null,
        bool? isStarted = null,  bool? isFinished = null, bool? isError = null,
        CancellationToken cancellationToken = default)
    {
        var execution = await GetResultAsync(guid, cancellationToken) 
                        ?? throw new ItemNotFountException(nameof(CodeExecution), guid);

        if (cancellationToken.IsCancellationRequested)
            return execution;

        if (isError is not null)
            execution.IsError = isError.Value;
        if (isError == true || isFinished == true)
            execution.FinishedAt = DateTimeOffset.Now;
        if (isStarted == true)
            execution.StartedAt = DateTimeOffset.Now;
        if (comment is not null)
            execution.Comment = comment;
        if (data is not null)
        {
            execution.Result ??= new CodeExecutionResult();
            execution.Result.Data += data;   
        }
        
        execution.UpdatedAt = DateTimeOffset.Now;
        
        return execution;
    }

    public async Task Delete(Guid guid, CancellationToken cancellationToken = default)
    {
        var execution = await dbSet.FindAsync(guid, cancellationToken);
        if (execution is not null) 
            dbSet.Remove(execution);
    }
}