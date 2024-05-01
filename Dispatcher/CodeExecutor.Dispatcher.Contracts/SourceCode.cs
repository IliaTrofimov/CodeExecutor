using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>
/// Source code view.
/// </summary>
public sealed class SourceCode
{
    /// <summary>Guid of code execution process.</summary>
    [Required]
    public Guid Guid { get; set; }
    
    /// <summary>Source text.</summary>
    public string CodeText { get; set; } = null!;
    
    /// <summary>Programming language Id.</summary>
    [Required]
    public long LanguageId { get; set; } 
}