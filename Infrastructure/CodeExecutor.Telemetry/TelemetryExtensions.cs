using System.Diagnostics;


namespace CodeExecutor.Telemetry;

public static class TelemetryExtensions
{
    public static void AddEvent(this Activity? activity, string name, params (string tagName, object tagValue)[] tags)
    {
        if (activity is null) return;
        
        var tagsCollection = new ActivityTagsCollection(
            tags.Select(t => new KeyValuePair<string, object?>(t.tagName, t.tagValue)));

        activity.AddEvent(new ActivityEvent(name, tags: tagsCollection));
    }
    
    public static void AddEvent(this Activity? activity, string name, string tagName, object tagValue)
    {
        if (activity is null) return;

        var tagsCollection = new ActivityTagsCollection()
        {
            new KeyValuePair<string, object?>(tagName, tagValue)
        };
        activity.AddEvent(new ActivityEvent(name, tags: tagsCollection));
    }
}