using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>
/// Programming language.
/// </summary>
public sealed class Language
{
    /// <summary>Programming language Id.</summary>
    [Required]
    public long Id { get; set; }
    
    /// <summary>Programming language name.</summary>
    [Required]
    public string Name { get; set; } = null!;
    
    /// <summary>Programming language version.</summary>
    public string? Version { get; set; } 
}