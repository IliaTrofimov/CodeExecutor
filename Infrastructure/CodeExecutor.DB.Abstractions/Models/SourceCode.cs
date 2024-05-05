using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.DB.Abstractions.Models;

/// <summary>Code execution source code.</summary>
public class SourceCode : BaseEntity<Guid>
{
    /// <summary>Source code of some code execution request.</summary>
    public string CodeText { get; set; } = "";

    /// <summary>Code execution.</summary>
    [Required]
    public virtual CodeExecution CodeExecution { get; set; } = null!;
}