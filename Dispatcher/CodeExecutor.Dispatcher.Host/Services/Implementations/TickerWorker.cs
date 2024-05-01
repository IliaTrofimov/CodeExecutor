namespace CodeExecutor.Dispatcher.Host.Services.Implementations;

public class TickerWorker(ILogger<TickerWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("CodeExecutor.Dispatcher.Host TickerWorker is working");
        const int ticks = 5;
        for (var i = 1; !stoppingToken.IsCancellationRequested && i <= ticks; i++)
        {
            logger.LogInformation("CodeExecutor.Dispatcher.Host is working, tick={tick}/{maxTick}", i, ticks);
            await Task.Delay(2000 + 500*i*i, stoppingToken);
        }
        logger.LogInformation("CodeExecutor.Dispatcher.Host TickerWorker is deleted");
    }
}