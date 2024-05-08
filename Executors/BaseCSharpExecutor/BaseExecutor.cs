using System.Text;
using Microsoft.Extensions.Logging;

using BaseCSharpExecutor.Api;
using CodeExecutor.Dispatcher.Contracts;


namespace BaseCSharpExecutor;


/// <summary>
/// Base code executor class.
/// </summary>
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
        var resultTasks = Process(startMessage)
            .Select(res => dispatcherClient.SetResultAsync(res, startMessage.ValidationTag));
        
        return Task.WhenAll(resultTasks);
    }
    
    
    protected IEnumerable<CodeExecutionResult> Process(ExecutionStartMessage startMessage)
    {
        var helper = new ExecutionResultHelper(startMessage.Guid);
        
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
            else
            {
                logger.LogInformation("Execution {executionId} finished successfully", helper.Guid);
                return helper.GetSuccess(scriptOutput.ToString());
            }
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