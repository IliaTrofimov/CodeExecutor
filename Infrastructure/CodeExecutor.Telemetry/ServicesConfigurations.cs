using System.Diagnostics;
using System.Reflection;
using CodeExecutor.Common.Models.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace CodeExecutor.Telemetry;

public static class ServicesConfigurations
{
    /// <summary>
    /// Configure OTLP tracing and metrics from given configuration.
    /// </summary>
    public static void AddTelemetry(this IServiceCollection services, IConfiguration config, string? serviceName = null)
    {
        var telemetryConfig = config.GetSection("Telemetry");
        if (!telemetryConfig.Exists()) return;

        serviceName ??= (Assembly.GetCallingAssembly().GetName().Name ?? "Service")
            .Replace(".Host", "");

        SetupTelemetry(services, telemetryConfig, serviceName);
        SetupMetrics(services, telemetryConfig, serviceName);

        TelemetryProvider.Create(serviceName);
    }
    

    private static void SetupTelemetry(IServiceCollection services, IConfiguration config, string serviceName)
    {
        var tracingConfig = config.GetSection("Tracing");
        if (!tracingConfig.Exists()) return;

        var url = tracingConfig.GetValue("ExporterUrl");
        var useHttpTracing = UseInstrumentation(tracingConfig, "UseHttpTracing", false);
        var useSqlTracing = UseInstrumentation(tracingConfig, "UseSqlTracing", false);

        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(serviceName);
            builder.AddAspNetCoreInstrumentation();
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri(url)
            );
            builder.ConfigureResource(builder =>
            {
                builder.AddService(serviceName, serviceVersion: "1.0.0");
                builder.AddEnvironmentVariableDetector();
            });

            if (useSqlTracing)
            {
                builder.AddEntityFrameworkCoreInstrumentation(options =>
                {
                    options.EnrichWithIDbCommand = (activity, command) =>
                    {
                        activity.DisplayName = command.CommandText.Split()[0];
                        activity.SetTag("db.commandText", command.CommandText);
                    };
                });
                builder.AddSqlClientInstrumentation();  
            }

            if (useHttpTracing)
                builder.AddHttpClientInstrumentation(options => options.RecordException = true);
        });
    }

    private static void SetupMetrics(IServiceCollection services, IConfiguration config, string serviceName)
    {
        var metricsConfig = config.GetSection("Metrics");
        if (!metricsConfig.Exists()) return;

        var url = metricsConfig.GetValue("ExporterUrl");

        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            builder.AddAspNetCoreInstrumentation();
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri(url)
            );

            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion: "1.0.0")
            );

            builder.AddHttpClientInstrumentation();
        });
    }

    private static bool UseInstrumentation(IConfiguration configuration, string path, bool defaultValue)
    {
        var exists = configuration.TryGetValue(path, out var useString);
        if (!exists) return defaultValue;

        return bool.TryParse(useString, out var useValue) ? useValue : defaultValue;
    }
}