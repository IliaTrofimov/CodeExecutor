using System.Diagnostics;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Telemetry;


namespace BaseCSharpExecutor;

public static class TracingHelper
{
    /// <summary>
    /// Add "Yield result" event.
    /// </summary>
    public static void AddExecutionResultEvent(CodeExecutionResult result)
    {
        if (Activity.Current is null) return;
        
        var tags = new ActivityTagsCollection
        {
            new("execution.Status", result.Status),
            new("execution.IsError", result.IsError)
        };
        Activity.Current.AddEvent(new ActivityEvent("Yield result", tags: tags));
    }
    
    public static void AddExecutionStartedEvent(Type executorType)
    {
        if (Activity.Current is null) return;
        
        var tags = new ActivityTagsCollection
        {
            new("executor.Type", executorType.FullName),
        };
        Activity.Current.AddEvent(new ActivityEvent("Internal execution started", tags: tags));
    }
    
    public static void AddExecutionFinishedEvent(Type executorType, int internalErrorsCount)
    {
        if (Activity.Current is null) return;
        
        var tags = new ActivityTagsCollection
        {
            new("executor.Type", executorType.FullName),
            new("execution.InternalErrors", internalErrorsCount)
        };
        Activity.Current.AddEvent(new ActivityEvent("Internal execution finished", tags: tags));
    }
}