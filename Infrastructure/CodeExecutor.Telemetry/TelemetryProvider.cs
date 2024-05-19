using System.Diagnostics;


namespace CodeExecutor.Telemetry;

public class TelemetryProvider
{
    private static TelemetryProvider? instance = null;
    private static ActivitySource? source = null;
    public static ActivitySource Source => source;
    
    public static Activity? Current => Activity.Current;
    
    
    private TelemetryProvider(string serviceName)
    {
        source = new ActivitySource(serviceName);
    }

    public static TelemetryProvider Create(string serviceName) => instance ?? new TelemetryProvider(serviceName);


    public static Activity? StartNew(string name, ActivityKind activityKind = ActivityKind.Internal) 
        => Source?.StartActivity(name, activityKind);

    public static Activity? StartNew(string name, string? parentId, ActivityKind activityKind = ActivityKind.Internal)
    {
        return Source?.StartActivity(name, activityKind, parentId);
    }
        
    
    public static void AddTag(string name, object value) => Current?.AddTag(name, value);

    public static void AddEvent(string name) => Current?.AddEvent(new ActivityEvent(name));

    public static void AddEvent(string name, params (string tagName, object tagValue)[] tags)
    {
        if (Current is null) return;
        
        var tagsCollection = new ActivityTagsCollection(
            tags.Select(t => new KeyValuePair<string, object?>(t.tagName, t.tagValue)));

        Current?.AddEvent(new ActivityEvent(name, tags: tagsCollection));
    }
}