using System.Diagnostics;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging.Abstractions;
using CodeExecutor.Messaging.Services;
using CodeExecutor.Telemetry;
using Microsoft.Extensions.Logging;


namespace BaseCSharpExecutor;

/// <summary>RabbitMQ message receiver for handling ExecutionStartMessages.</summary>
public class ExecutionMessageReceiver : BasicMessageReceiver<ExecutionStartMessage>
{
    private readonly BaseExecutor executor;

    public ExecutionMessageReceiver(BaseExecutor executor, IMessageReceiverConfig config,
                                    ILogger<BasicMessageSender> logger)
        : base(config, logger)
    {
        this.executor = executor;
    }

    public override async void HandleMessage(ExecutionStartMessage message)
    {
        using var activity = TraceRoot.Start("Receive execution", message.TraceId, ActivityKind.Consumer);   
        
        try
        {
            activity?.AddTag("execution.Id", message.Guid);
            await executor.StartExecution(message);
        }
        catch (Exception ex)
        {
            logger.LogCritical("Unhandled exception {errorType} while handling message:\n{error}",
                ex.GetType(), ex);

            AddErrorEvent(activity, ex);
        }

        await Task.Yield();
    }


    private static void AddErrorEvent(Activity? activity, Exception exception)
    {
        if (activity is null) return;

        var tags = new ActivityTagsCollection
        {
            new("exception.Type", exception.GetType().Name),
            new("exception.Message", exception.Message),
            new("exception.Trace", exception.StackTrace)
        };
        
        activity.AddEvent(new ActivityEvent("Unhandled exception", tags: tags)); 
    }
}