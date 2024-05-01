using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Host.Services.Interfaces;
using CodeExecutor.Messaging;


namespace CodeExecutor.Dispatcher.Host.Services.Implementations;

public sealed class CodeExecutionMq : BasicMessageSender, ICodeExecutionMessaging
{
    private const string Exchange = "executions";
    
    public CodeExecutionMq(MessagingConfig config, ILogger<CodeExecutionMq> logger) 
        : base(config, logger)
    {
    }

    public Task SendStartMessageAsync(CodeExecutionExpanded codeExecution, string executionKey, ExecutionPriority priority = ExecutionPriority.Normal)
    {
        var queue = $"{codeExecution.Language.Name}-{codeExecution.Language.Version ?? "0"}";
        
        var message = new ExecutionStartMessage
        {   
            Guid = codeExecution.Guid,
            LanguageId = codeExecution.Language.Id,
            Priority = priority,
            SourceCode = codeExecution.SourceCode ?? "",
            ValidationTag = executionKey
        };

        return SendAsync(message, queue.ToLower(), Exchange, (byte)priority);
    }
}