using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Short info about code execution.</summary>
public class CodeExecution
{
    /// <summary>Guid of code execution process.</summary>
    [Required]
    public Guid Guid { get; set; }

    /// <summary>Programming language that is executing this request.</summary>
    [Required]
    public Language Language { get; set; } = null!;

    /// <summary>Request time.</summary>
    [Required]
    public DateTimeOffset RequestedAt { get; set; }

    /// <summary>Execution start time.</summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>Execution finish time.</summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary>Execution update time.</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Additional data about this execution.</summary>
    public string? Comment { get; set; }

    /// <summary>Execution has finished with error.</summary>
    [Required]
    public bool IsError { get; set; }
}