using CodeExecutor.DB.Abstractions.Repository;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeExecutor.Auth.Host.Services;

public class HealthCheckService : IHealthCheck
{
    private readonly ILogger<HealthCheckService> logger;
    private readonly IUsersRepository usersRepository;

    
    public HealthCheckService(ILogger<HealthCheckService> logger, 
        IUsersRepository usersRepository)
    {
        this.logger = logger;
        this.usersRepository = usersRepository;
    }
    
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Health check started");
        var result = await CheckDb();
        return result is not null 
            ? HealthCheckResult.Unhealthy(result) 
            : HealthCheckResult.Healthy();
    }


    private async Task<string?> CheckDb()
    {
        try
        {
            await usersRepository.CountAsync();
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
}