using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Class that contains Guid of newly created code execution and some additional comment.</summary>
public sealed class CodeExecutionStartResponse
{
    /// <summary>Guid of code execution process.</summary>
    [Required]
    public Guid Guid { get; set; }

    /// <summary>Additional info about given execution.</summary>
    public string? Comment { get; set; }
    
    /// <summary>Open telemetry trace id.</summary>
    public string? TraceId { get; set; }
}