using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.DB.Models;

/// <summary>Code execution result data.</summary>
public class CodeExecutionResult : BaseEntity<Guid>
{
    /// <summary>Serialized code execution result.</summary>
    public string? Data { get; set; }

    /// <summary>Code execution.</summary>
    [Required] public CodeExecution CodeExecution { get; set; } = null!;
}