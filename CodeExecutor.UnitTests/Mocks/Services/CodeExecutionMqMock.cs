using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Dispatcher.Host.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CodeExecutor.UnitTests.Mocks.Services;

public class CodeExecutionMqMock : ICodeExecutionMessaging
{
    protected readonly ILogger? Logger;
    
    public Action<CodeExecutionExpanded, string>? OnSendStartMessage { get; set; }


    public CodeExecutionMqMock(ILogger? logger = null) => Logger = logger;
    
    
    public Task SendStartMessageAsync(CodeExecutionExpanded codeExecution, string executionKey,
        ExecutionPriority priority = ExecutionPriority.Normal)
    {
        Logger?.LogDebug($"MOCK {nameof(SendStartMessageAsync)}");
        
        Assert.False(string.IsNullOrWhiteSpace(executionKey), 
            $"CodeExecution {nameof(executionKey)} cannot be empty at {nameof(SendStartMessageAsync)}()");
        Assert.NotEqual(new Guid(), codeExecution.Guid);
        
        SendStartMessageCallback(codeExecution, executionKey);
        
        return Task.CompletedTask;
    }

    private void SendStartMessageCallback(CodeExecutionExpanded codeExecution, string executionKey)
    {
        if (OnSendStartMessage is null) return;
        Logger?.LogDebug($"MOCK Callback {nameof(SendStartMessageCallback)}");
        OnSendStartMessage.Invoke(codeExecution, executionKey);
    }
}