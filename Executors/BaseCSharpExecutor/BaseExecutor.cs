using System.Diagnostics;
using System.Text;
using BaseCSharpExecutor.Api;
using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.Telemetry;
using Microsoft.Extensions.Logging;


namespace BaseCSharpExecutor;

/// <summary>Base code executor class.</summary>
public abstract class BaseExecutor
{
    private readonly ICodeExecutionDispatcherClient dispatcherClient;
    private readonly ILogger<BaseExecutor> logger;


    protected BaseExecutor(ICodeExecutionDispatcherClient dispatcherClient, ILogger<BaseExecutor> logger)
    {
        this.dispatcherClient = dispatcherClient;
        this.logger = logger;
    }

    protected abstract void RunScriptInternal(string sourceCode, Func<Exception, bool> exceptionHandler);


    /// <summary>Start code execution.</summary>
    public Task StartExecution(ExecutionStartMessage startMessage)
    {
        var tasks = new List<Task>();
        foreach (var result in Process(startMessage))
        {
            TelemetryProvider.AddEvent("Yield result", 
                ("execution.Status", result.Status),
                ("execution.IsError", result.IsError ?? false));
            tasks.Add(dispatcherClient.SetResultAsync(result, startMessage.ValidationTag));
        }
        Activity.Current?.Stop();
        return Task.WhenAll(tasks);
    }


    protected IEnumerable<CodeExecutionResult> Process(ExecutionStartMessage startMessage)
    {
        var helper = new ExecutionResultHelper(startMessage.Guid);
        using var activity = TelemetryProvider.StartNew("Script execution");

        if (string.IsNullOrWhiteSpace(startMessage.SourceCode))
        {
            logger.LogInformation("Execution '{executionId}' has empty source code", startMessage.Guid);
            yield return helper.GetError("Execution has empty source code");
            yield break;
        }

        logger.LogInformation("Execution '{executionId}' is started", startMessage.Guid);
        yield return helper.GetStarted();
        
        yield return RunScript(startMessage.SourceCode, helper);
    }

    protected CodeExecutionResult RunScript(string sourceCode, ExecutionResultHelper helper)
    {
        var (scriptOutput, standardOutput) = CreateScriptWriter();

        var errorsSb = new StringBuilder();
        var errorsCount = 0;

        bool CatchScriptException(Exception ex)
        {
            errorsSb.AppendLine($"Unhandled exception:\n{ex.Message}\n{ex.StackTrace}\n\n");
            errorsCount++;
            return true;
        }

        try
        {
            RunScriptInternal(sourceCode, CatchScriptException);
            Console.SetOut(standardOutput);

            if (errorsCount > 0)
            {
                logger.LogWarning("Execution {executionId} finished with {errorsCount} errors",
                    helper.Guid, errorsCount);

                return helper.GetError(errorsCount, $"{scriptOutput}\n\n{errorsSb}");
            }

            logger.LogInformation("Execution {executionId} finished successfully", helper.Guid);
            return helper.GetSuccess(scriptOutput.ToString());
        }
        catch (Exception ex)
        {
            Console.SetOut(standardOutput);
            logger.LogError("Execution {executionId} finished with unhandled error {errorType}:\n{error}",
                helper.Guid, ex.GetType(), ex);

            return helper.GetError($"Execution finished with unhandled error {ex.GetType()}");
        }
    }

    protected static (StringWriter scriptOutput, StreamWriter standardOutput) CreateScriptWriter()
    {
        var scriptOutput = new StringWriter();
        var standardOutput = new StreamWriter(Console.OpenStandardOutput());
        Console.SetOut(scriptOutput);

        return (scriptOutput, standardOutput);
    }
}