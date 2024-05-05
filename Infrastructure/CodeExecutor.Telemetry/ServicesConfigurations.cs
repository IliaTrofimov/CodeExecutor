using System.Reflection;
using CodeExecutor.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace CodeExecutor.Telemetry;

public static class ServicesConfigurations
{
    public static void AddTelemetry(this IServiceCollection services, IConfiguration config)
    {
        var telemetryConfig = config.GetSection("Telemetry");
        if (!telemetryConfig.Exists()) return;

        var tracingConfig = telemetryConfig.GetSection("Tracing");
        var tracingEnabled = tracingConfig.Exists();
        
        var metricsConfig = telemetryConfig.GetSection("Metrics");
        var metricsEnabled = tracingConfig.Exists();
        
        if (tracingEnabled)
        {
            var uri = new Uri(tracingConfig.GetValue("Host"));
            
            services.AddOpenTelemetry().WithTracing(builder =>
            {
                builder.AddSqlClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddOtlpExporter(options =>
                    options.Endpoint = uri
                );
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(Assembly.GetCallingAssembly().GetName().Name.Replace(".Host", ""))
                );
            });
        }

        if (metricsEnabled)
        {
            services.AddOpenTelemetry().WithMetrics(builder =>
            {
                
            });
        }
    }
}