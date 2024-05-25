using CodeExecutor.Common.Models.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


namespace CodeExecutor.Telemetry;

public static class ServicesConfigurations
{
    /// <summary>
    /// Configure OTLP tracing and metrics from given configuration.
    /// </summary>
    public static void AddTelemetry(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        var telemetryConfig = config.GetSection("Telemetry");
        if (!telemetryConfig.Exists()) return;
        
        var serviceInstance = config.GetValue<string>("ServiceInstance");
        serviceInstance = serviceInstance is not null ? $"1.0.0-{serviceInstance}" : "1.0.0";
        
        SetupTelemetry(services, telemetryConfig, env.ApplicationName, serviceInstance);
        SetupMetrics(services, telemetryConfig, env.ApplicationName, serviceInstance);

        TraceRoot.Create(env.ApplicationName, serviceInstance);
    }
    

    private static void SetupTelemetry(IServiceCollection services, IConfiguration config, string serviceName, string serviceVersion)
    {
        var tracingConfig = config.GetSection("Tracing");
        if (!tracingConfig.Exists()) return;

        var url = tracingConfig.GetValue("ExporterUrl");
        var useHttpTracing = UseInstrumentation(tracingConfig, "UseHttpTracing", false);
        var useSqlTracing = UseInstrumentation(tracingConfig, "UseSqlTracing", false);
        var useConsoleTracing = UseInstrumentation(tracingConfig, "UseConsoleTracing", false);

        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(serviceName);
            builder.AddAspNetCoreInstrumentation();
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri(url)
            );
            builder.ConfigureResource(options =>
            {
                options.AddService(serviceName, serviceVersion: serviceVersion);
                options.AddEnvironmentVariableDetector();
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
                builder.AddHttpClientInstrumentation();

            if (useConsoleTracing)
                builder.AddConsoleExporter();
        });
    }
    
    private static void SetupMetrics(IServiceCollection services, IConfiguration config, string serviceName, string serviceVersion)
    {
        var metricsConfig = config.GetSection("Metrics");
        if (!metricsConfig.Exists()) return;

        var url = metricsConfig.GetValue("ExporterUrl");

        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri(url)
            );
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion: serviceVersion)
            );
            
            builder.AddAspNetCoreInstrumentation();
            builder.AddPrometheusExporter();
            
            builder.AddMeter("Microsoft.AspNetCore.Hosting");
            builder.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
            builder.AddMeter("Microsoft.AspNetCore.Hosting");
            builder.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
            builder.AddMeter("Microsoft.AspNetCore.Http.Connections");
            builder.AddMeter("Microsoft.AspNetCore.Routing");
            builder.AddMeter("Microsoft.AspNetCore.Diagnostics");
            builder.AddMeter("Microsoft.AspNetCore.RateLimiting");
        });
    }

    private static bool UseInstrumentation(IConfiguration configuration, string path, bool defaultValue)
    {
        var exists = configuration.TryGetValue(path, out var useString);
        if (!exists) return defaultValue;

        return bool.TryParse(useString, out var useValue) ? useValue : defaultValue;
    }
}