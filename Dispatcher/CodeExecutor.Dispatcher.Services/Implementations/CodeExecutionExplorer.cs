using AutoMapper;
using CodeExecutor.DB.Repository;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Services.Interfaces;

namespace CodeExecutor.Dispatcher.Services.Implementations;

public sealed class CodeExecutionExplorer : ICodeExecutionExplorer
{
    private readonly DbRepository.ICodeExecutionsExplorerRepository viewRepository;
    private readonly IMapper mapper;
    

    public CodeExecutionExplorer(DbRepository.ICodeExecutionsExplorerRepository viewRepository,
        IMapper mapper)
    {
        this.viewRepository = viewRepository;
        this.mapper = mapper;
    }


    public async Task<CodeExecutionExpanded?> GetExecutionResultAsync(Guid executionGuid, long userId)
    {
        var execution = await viewRepository.GetResultAsync(executionGuid);
        if (execution is null)
            return null;
        if (execution.InitiatorId != userId)
            throw new UnauthorizedException("Cannot view this execution.");

        return mapper.Map<CodeExecutionExpanded>(execution);
    }

    public async Task<SourceCode?> GetSourceCodeAsync(Guid executionGuid, long userId)
    {
        var execution = await viewRepository.GetSourceCodeAsync(executionGuid);
        if (execution is null)
            return null;
        if (execution.InitiatorId != userId)
            throw new UnauthorizedException("Cannot view this execution.");
        
        return mapper.Map<SourceCode>(execution);
    }

    public async Task<List<CodeExecution>> GetExecutionsListAsync(long userId, int? skip = null, int? take = null, IEnumerable<Guid>? guids = null)
    {
        var query = viewRepository.Query().Where(e => e.InitiatorId == userId);
        
        if (guids is not null)
            query = query.Where(e => guids.Contains(e.Id));
        
        query = query.OrderByDescending(e => e.RequestedAt)
            .ThenByDescending(e => e.UpdatedAt)
            .Skip(skip ?? 0)
            .Take(take ?? int.MaxValue);

        var executions = await query.ToListAsync();
        return mapper.Map<List<CodeExecution>>(executions)!;
    }
}