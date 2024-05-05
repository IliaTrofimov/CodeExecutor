using AutoMapper;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Services.Interfaces;
using Microsoft.Extensions.Logging;


namespace CodeExecutor.Dispatcher.Services.Implementations;

public sealed class CodeExecutionDispatcher : ICodeExecutionDispatcher
{
    private readonly DbRepository.ICodeExecutionsExplorerRepository viewRepository;
    private readonly DbRepository.ICodeExecutionsEditorRepository editRepository;
    private readonly DbRepository.ILanguagesRepository languagesRepository;
    private readonly ICodeExecutionMessaging messaging;
    private readonly ILogger<CodeExecutionDispatcher> logger;
    private readonly IMapper mapper;

    
    public CodeExecutionDispatcher(
        DbRepository.ICodeExecutionsExplorerRepository viewRepository,
        DbRepository.ICodeExecutionsEditorRepository editRepository,
        DbRepository.ILanguagesRepository languagesRepository,
        ICodeExecutionMessaging messaging,
        ILogger<CodeExecutionDispatcher> logger,
        IMapper mapper)
    {
        this.viewRepository = viewRepository;
        this.editRepository = editRepository;
        this.languagesRepository = languagesRepository;
        this.messaging = messaging;
        this.mapper = mapper;
        this.logger = logger;
    }
    
    public async Task<CodeExecutionStartResponse> StartCodeExecutionAsync(CodeExecutionRequest request, long userId)
    {
        var dbLanguage = await languagesRepository.GetAsync(request.LanguageId)
                   ?? throw new BadRequestException($"Language with id {request.LanguageId} not exists");

        var (execution, secret) = await CreateCodeExecution(request, userId);
        execution.Language = mapper.Map<Language>(dbLanguage)!;

        await SendMessage(execution, request.Priority, secret);
        logger.LogInformation("Execution {executionId} is waiting in queue", execution.Guid);
        
        return new CodeExecutionStartResponse
        {
            Guid = execution.Guid,
            Comment = "Waiting in queue"
        };
    }

    public async Task DeleteCodeExecutionAsync(Guid executionGuid, long userId)
    {
        var execution = await viewRepository.GetAsync(executionGuid);
        if (execution is null)
            return;

        if (execution.InitiatorId != userId)
            throw new UnauthorizedException("Cannot delete this code execution.");

        await editRepository.Delete(executionGuid);
        await editRepository.SaveAsync();
    }

    
    public async Task SetExecutionResultsAsync(CodeExecutionResult result, string validationTag)
    {
        if (result.Data is null && result.Comment is null && result.IsError is null && result.Status is null)
        {
            logger.LogInformation("CodeExecution {executionId} was not updated: empty update request", result.Guid);
            return;
        }
            
        if (!await viewRepository.CheckSecretKeyAsync(result.Guid, validationTag))
            throw new UnauthorizedException("Cannot change this code execution.");

        var task = result.Status switch 
        {
            CodeExecutionStatus.Started => editRepository.SetResult(result.Guid, result.Data, result.Comment, isStarted: true),
            CodeExecutionStatus.Finished => editRepository.SetResult(result.Guid, result.Data, result.Comment, isFinished: true),
            CodeExecutionStatus.Error => editRepository.SetResult(result.Guid, result.Data, result.Comment, isError: true),
            _ => editRepository.SetResult(result.Guid, result.Data, result.Comment),
        };
        await task;
        await editRepository.SaveAsync();
        
        logger.LogInformation("CodeExecution {executionId} was updated:\nstatus={status}, comment={comment}",
            result.Guid, result.Status, result.Comment);
    }

    
    private async Task<(CodeExecutionExpanded, string)> CreateCodeExecution(CodeExecutionRequest request, long userId)
    {
        try
        {
            var execution = await editRepository.Create(userId, request.LanguageId, request.CodeText);
            await editRepository.SaveAsync();
            return (mapper.Map<CodeExecutionExpanded>(execution)!, execution.SecretKey);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Cannot start code execution: database error.", ex);
        }
    }

    private async Task SendMessage(CodeExecutionExpanded execution, ExecutionPriority priority, string msgValidationTag)
    {
        try
        {
             await messaging.SendStartMessageAsync(execution, msgValidationTag, priority);
        }
        catch (Exception ex)
        {
            try
            {
                await editRepository.Delete(execution.Guid);
                await editRepository.SaveAsync();
            }
            finally
            {
                throw new InfrastructureException($"Execution instantiation error ({ex.Message})", ex);
            }
        }
    }
}