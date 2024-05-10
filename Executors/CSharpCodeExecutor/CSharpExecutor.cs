using BaseCSharpExecutor;
using BaseCSharpExecutor.Api;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;


namespace CSharpCodeExecutor;

public class CSharpExecutor : BaseExecutor
{
    public CSharpExecutor(ICodeExecutionDispatcherClient dispatcherClient, ILogger<BaseExecutor> logger)
        : base(dispatcherClient, logger)
    {
    }

    protected override void RunScriptInternal(string sourceCode, Func<Exception, bool> exceptionHandler)
    {
        Script<object>? script = CSharpScript.Create(sourceCode, ScriptOptions.Default);
        script.RunAsync(null, exceptionHandler).Wait();
    }
}