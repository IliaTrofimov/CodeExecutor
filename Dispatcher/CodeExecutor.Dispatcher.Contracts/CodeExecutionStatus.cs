namespace CodeExecutor.Dispatcher.Contracts;

/// <summary>Status of code execution.</summary>
public enum CodeExecutionStatus { None = 0, Created, Pending, Started, Error, Finished }