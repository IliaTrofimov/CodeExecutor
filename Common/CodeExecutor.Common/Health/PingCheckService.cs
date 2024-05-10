using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;


namespace CodeExecutor.Common.Health;

public class PingCheckService(ILogger logger) : IHealthCheck
{
    public virtual Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                    CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Ping");
        return Task.FromResult(HealthCheckResult.Healthy($"Response at {DateTimeOffset.Now}"));
    }
}


public static class PingHealthCheckExtensions
{
    public static void AddPingHealthCheck(this WebApplication app, string url = "/ping")
    {
        app.MapGet(url, () => $"Response at {DateTimeOffset.Now}");
    }
} 