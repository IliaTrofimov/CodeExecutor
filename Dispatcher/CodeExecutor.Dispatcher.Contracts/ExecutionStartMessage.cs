using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Execution request rabbit message.</summary>
public sealed class ExecutionStartMessage
{
    /// <summary>Guid of request to be executed.</summary>
    [Required]
    public Guid Guid { get; set; }

    /// <summary>Programming language that will execute this request.</summary>
    [Required]
    public long LanguageId { get; set; }

    /// <summary>Code execution priority.</summary>
    [Required]
    public ExecutionPriority Priority { get; set; }

    /// <summary>Source code data.</summary>
    [Required]
    public string SourceCode { get; set; } = null!;

    /// <summary>Validation tag required for code execution modification.</summary>
    [Required]
    public string ValidationTag { get; set; } = null!;
    
    /// <summary>Open telemetry trace id.</summary>
    public string? TraceId { get; set; }
}