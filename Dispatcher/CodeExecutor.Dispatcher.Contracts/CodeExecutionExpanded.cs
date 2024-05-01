namespace CodeExecutor.Dispatcher.Contracts;


/// <summary>
/// Class that contains final result of some code execution.
/// </summary>
public sealed class CodeExecutionExpanded : CodeExecution
{
    /// <summary>Payload.</summary>
    public string? Data { get; set; }
    
    /// <summary>Code execution source code.</summary>
    public string? SourceCode { get; set; }
}