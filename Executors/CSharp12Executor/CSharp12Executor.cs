using BaseCSharpExecutor;
using BaseCSharpExecutor.Api;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CSharp12Executor;


public class CSharp12Executor : BaseExecutor
{

    public CSharp12Executor(ICodeExecutionDispatcherClient dispatcherClient, ILogger<BaseExecutor> logger) 
        : base(dispatcherClient, logger)
    {
    }

    protected override void RunScriptInternal(string sourceCode, Func<Exception, bool> exceptionHandler)
    {
        var script = CSharpScript.Create(sourceCode, ScriptOptions.Default);
        script.RunAsync(null, exceptionHandler).Wait();
    }
}