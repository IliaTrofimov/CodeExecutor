using System.Diagnostics;


namespace CodeExecutor.Telemetry;

public static class TraceRoot
{
    public static ActivitySource Root { get; private set; } = null!;
    
    

    public static void Create(string serviceName, string? version = null)
    {
        Root ??= new ActivitySource(serviceName, version);
    }

    public static Activity? Start(string name, ActivityKind activityKind = ActivityKind.Internal)
    {
        return Root.StartActivity(name, activityKind);
    }

    public static Activity? Start(string name, string? parentId, ActivityKind activityKind = ActivityKind.Internal)
    {
        return Root.StartActivity(name, activityKind, parentId);
    }
}