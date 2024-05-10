using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace CodeExecutor.Dispatcher.Host;

public class HealthCheckService : IHealthCheck
{
    private readonly ILogger<HealthCheckService> logger;
    private readonly DbRepository.ILanguagesRepository languagesRepository;


    public HealthCheckService(ILogger<HealthCheckService> logger,
                              DbRepository.ILanguagesRepository languagesRepository)
    {
        this.logger = logger;
        this.languagesRepository = languagesRepository;
    }


    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                          CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Health check started");
        Task<string?> dbTask = CheckDb();
        Task<string?> mqTask = CheckMq();

        var results = await Task.WhenAll(dbTask, mqTask);
        if (results[0] is not null || results[1] is not null)
            return HealthCheckResult.Unhealthy(string.Join(". ", results));

        return HealthCheckResult.Healthy();
    }


    private async Task<string?> CheckDb()
    {
        try
        {
            await languagesRepository.CountAsync();
            logger.LogDebug("Health check {healthCheckType}: {healthCheckResult}",
                "database", "healthy");

            return null;
        }
        catch
        {
            logger.LogWarning("Health check {healthCheckType}: {healthCheckResult}",
                "database", "unhealthy");

            return "Database is inaccessible";
        }
    }

    private Task<string?> CheckMq()
    {
        logger.LogDebug("Health check {healthCheckType}: {healthCheckResult}",
            "message queue", "healthy");

        return Task.FromResult<string?>(null);
    }
}