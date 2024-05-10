using System.ComponentModel.DataAnnotations;


namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Request class</summary>
public sealed class CodeExecutionRequest
{
    /// <summary>Source code on given programming language that will be executed.</summary>
    [Required]
    public string CodeText { get; set; } = null!;

    /// <summary>Programming language that will execute this request.</summary>
    [Required]
    public long LanguageId { get; set; }

    /// <summary>Code execution priority.</summary>
    public ExecutionPriority Priority { get; set; } = ExecutionPriority.Normal;
}