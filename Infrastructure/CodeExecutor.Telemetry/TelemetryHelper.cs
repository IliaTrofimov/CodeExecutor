using System.Diagnostics;
using System.Reflection;

namespace CodeExecutor.Telemetry;

/// <summary>
/// Helper class for working with OpenTelemetry activities.
/// </summary>
public static class TelemetryHelper
{
    /// <summary>Name of current service.</summary>
    public static readonly string ServiceName =
        Assembly.GetCallingAssembly().GetName().Name ?? "__service_root__";
    
    /// <summary>Root activity.</summary>
    public static readonly ActivitySource RootActivity = new(ServiceName);

    public static Activity? StartIncoming(string name)
        => RootActivity.StartActivity(name, ActivityKind.Server);
    
    public static Activity? StartOutgoing(string name)
        => RootActivity.StartActivity(name, ActivityKind.Client);
    
    public static Activity? StartConsumer(string name)
        => RootActivity.StartActivity(name, ActivityKind.Consumer);
    
    public static Activity? StartProducer(string name)
        => RootActivity.StartActivity(name, ActivityKind.Producer);

    public static Activity? StartInternal(string name)
        => RootActivity.StartActivity(name);
}