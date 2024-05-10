namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Code execution result object.</summary>
public sealed class CodeExecutionResult
{
    /// <summary>Guid of code execution.</summary>
    public Guid Guid { get; set; }

    /// <summary>Data that will be appended to result.</summary>
    public string? Data { get; set; }

    /// <summary>Comment.</summary>
    public string? Comment { get; set; }

    /// <summary>Is error.</summary>
    public bool? IsError { get; set; }

    /// <summary>Execution status.</summary>
    public CodeExecutionStatus? Status { get; set; }
}