using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging.Abstractions.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace BaseCSharpExecutor;

public class ExecutionWorker : BackgroundService
{
    private readonly IMessageReceiver<ExecutionStartMessage> messageReceiver;
    private readonly ILogger<ExecutionMessageReceiver> logger;

    public ExecutionWorker(IMessageReceiver<ExecutionStartMessage> messageReceiver,
                           ILogger<ExecutionMessageReceiver> logger)
    {
        this.messageReceiver = messageReceiver;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start worker service");
        messageReceiver.StartReceive();
    }
}