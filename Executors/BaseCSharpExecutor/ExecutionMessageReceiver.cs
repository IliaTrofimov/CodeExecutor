using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging.Abstractions;
using CodeExecutor.Messaging.Services;
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
        try
        {
            await executor.StartExecution(message);
        }
        catch (Exception ex)
        {
            logger.LogCritical("Unhandled exception {errorType} while handling message:\n{error}",
                ex.GetType(), ex);
        }

        await Task.Yield();
    }
}