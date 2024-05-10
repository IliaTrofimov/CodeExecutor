using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace CodeExecutor.Common.Logging;

public static class LoggingConfigurationExtensions
{
    public static void AddFileLogger(this IServiceCollection services, IWebHostEnvironment environment,
                                     string? logFileName = null)
    {
        services.AddLogging(b => b.AddProvider(new FileLoggerProvider(environment, logFileName)));
    }

    public static void AddConsoleLogger(this IServiceCollection services)
    {
        services.AddLogging(b => b.AddSimpleConsole(o => o.TimestampFormat = "HH:mm:ss "));
    }
}