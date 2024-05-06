#region

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

#endregion

namespace CodeExecutor.Telemetry;

public static class ServicesConfigurations
{
    public static void AddTelemetry(this IServiceCollection services, IConfiguration config)
    {
        var telemetryConfig = config.GetSection("Telemetry");
        if (!telemetryConfig.Exists()) return;

        var serviceName = (Assembly.GetCallingAssembly().GetName().Name ?? "Service")
            .Replace(".Host", "");

        SetupTelemetry(services, telemetryConfig, serviceName);
        SetupMetrics(services, telemetryConfig, serviceName);
    }

    private static void SetupTelemetry(IServiceCollection services, IConfiguration config, string serviceName)
    {
        var tracingConfig = config.GetSection("Tracing");
        if (!tracingConfig.Exists()) return;
        
        var host = tracingConfig.GetValue("Host");
        var port = tracingConfig.GetIntValue("Port");
        //var useHttpClient = !bool.TryParse(tracingConfig.GetValue("HttpInstrumentation"), out var parsed) || ;

        services.AddOpenTelemetry().WithTracing(builder =>
        {
            builder.AddSource(serviceName);
            builder.AddSqlClientInstrumentation();
            builder.AddAspNetCoreInstrumentation();
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri($"{host}:{port}")
            );
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion: "1.0.0")
            );
            builder.AddHttpClientInstrumentation(options =>
                options.RecordException = true
            );
        });
    }

    private static void SetupMetrics(IServiceCollection services, IConfiguration config, string serviceName)
    {
        var metricsConfig = config.GetSection("Metrics");
        if (!metricsConfig.Exists()) return;
        
        var host = metricsConfig.GetValue("Host");
        var port = metricsConfig.GetIntValue("Port");
        
        services.AddOpenTelemetry().WithMetrics(builder =>
        {
            builder.AddAspNetCoreInstrumentation();
            builder.AddOtlpExporter(options =>
                options.Endpoint = new Uri($"{host}:{port}")
            );
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(serviceName, serviceVersion: "1.0.0")
            );
            builder.AddHttpClientInstrumentation();
        });
    }
}