using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.DB.Abstractions.Models;

/// <summary>Code execution entity.</summary>
public class CodeExecution : BaseEntity<Guid>
{
    public CodeExecution()
    {
        RequestedAt = DateTimeOffset.Now;
        UpdatedAt = RequestedAt;

        var bytes = new byte[32];
        Random.Shared.NextBytes(bytes);
        SecretKey = Convert.ToHexString(bytes)[..16];
    }


    /// <summary>Execution priority.</summary>
    [Required]
    public int Priority { get; set; }

    /// <summary>Execution request time.</summary>
    [Required]
    public DateTimeOffset RequestedAt { get; set; }

    /// <summary>Execution update time.</summary>
    [Required]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Execution start time.</summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>Execution finish time.</summary>
    public DateTimeOffset? FinishedAt { get; set; }

    /// <summary>Additional data about this execution.</summary>
    [StringLength(1024)]
    public string? Comment { get; set; }

    /// <summary>Execution has finished with error.</summary>
    [Required]
    public bool IsError { get; set; }

    /// <summary>Secret key.</summary>
    [StringLength(32)]
    [Required]
    public string SecretKey { get; private set; }


    /// <summary>Code execution initiator Id.</summary>
    [Required]
    public long InitiatorId { get; set; }

    /// <summary>Programming language Id that has executed this request.</summary>
    [Required]
    public long LanguageId { get; set; }

    /// <summary>Programming language that has executed this request.</summary>
    [Required]
    public Language Language { get; set; } = null!;


    /// <summary>Result data.</summary>
    public virtual CodeExecutionResult? Result { get; set; }

    /// <summary>Source code that is being executed.</summary>
    public virtual SourceCode? SourceCode { get; set; }
}