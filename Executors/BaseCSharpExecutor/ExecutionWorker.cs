using BaseCSharpExecutor.Api;
using CodeExecutor.Common.Models.Configs;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Messaging.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace BaseCSharpExecutor;

public class ExecutionWorker : BackgroundService
{
    private readonly IMessageReceiver<ExecutionStartMessage> messageReceiver;
    private readonly ICodeExecutionDispatcherClient dispatcherClient;
    private readonly ILogger<ExecutionMessageReceiver> logger;
    private readonly bool runHealthChecks;

    public ExecutionWorker(IMessageReceiver<ExecutionStartMessage> messageReceiver,
                           ICodeExecutionDispatcherClient dispatcherClient,
                           ILogger<ExecutionMessageReceiver> logger,
                           IConfiguration config)
    {
        this.messageReceiver = messageReceiver;
        this.dispatcherClient = dispatcherClient;
        this.logger = logger;

        config.TryGetValue("HealthCheck:TestApi", out var str);
        bool.TryParse(str, out runHealthChecks);

        runHealthChecks = true;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start worker service");
        if (!await RunHealthChecks(stoppingToken))   
            throw new InfrastructureException();
        
        messageReceiver.StartReceive();
    }

    private async Task<bool> RunHealthChecks(CancellationToken stoppingToken)
    {
        if (!runHealthChecks)
        {
            logger.LogInformation("Health checks are disabled");
            return true;
        }

        logger.LogInformation("Start health checks");
        var result = await dispatcherClient.TryPingAsync(stoppingToken);
        if (result)
            logger.LogInformation("Health check is successful");
        else
            logger.LogError("Health check is unsuccessful. Shutdown");

        return result;
    }
    
}