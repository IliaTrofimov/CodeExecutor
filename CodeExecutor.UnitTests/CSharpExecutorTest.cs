using CodeExecutor.Dispatcher.Contracts;
using CodeExecutor.UnitTests.Mocks.Repositories;
using CodeExecutor.UnitTests.Mocks.Services;
using CSharp12Executor;
using Xunit.Abstractions;

namespace CodeExecutor.UnitTests;

public class CSharpExecutorTest : TestBase
{
    protected readonly CodeExecutionDispatcherApiMock DispatcherApiMock;
    protected readonly CSharpExecutor Executor;

    public CSharpExecutorTest(ITestOutputHelper output) : base(output)
    {
        DispatcherApiMock = new CodeExecutionDispatcherApiMock(new TestLogger<CodeExecutionDispatcherApiMock>(output));
        Executor = new CSharpExecutor(DispatcherApiMock, new TestLogger<CSharpExecutor>(output));
    }

    private static ExecutionStartMessage GetStartMessage(string? code = null)
    {
        return new ExecutionStartMessage
        {
            SourceCode = code ?? "for (int i = 0; i < 100; i++) System.Console.Write('+');",
            LanguageId = LanguagesRepositoryMock.CSharpId,
            ValidationTag = "validationTag",
            Guid = Guid.NewGuid()
        };
    }
    
    
    [Theory]
    [InlineData(10)]
    [InlineData(10000)]
    public async Task ExecuteLargeLoop(int maxLoop)
    {
        var execution = GetStartMessage($"for (int i = 0; i < {maxLoop}; i++) System.Console.Write('+');");

        DispatcherApiMock.OnSetResult = result =>
        {
            Assert.False(string.IsNullOrWhiteSpace(result.Data));
            Assert.Equal(maxLoop, result.Data.Length);
        };
        await Executor.StartExecution(execution);
    }
    
    [Fact]
    public async Task ExecuteNormal()
    {
        var execution = GetStartMessage();
        bool started = false, finished = false;

        DispatcherApiMock.OnSetResult = result =>
        {
            Assert.False(string.IsNullOrWhiteSpace(result.Data));
            Assert.Equal(100, result.Data.Length);
            finished = true;
        };
        DispatcherApiMock.OnStarted = result =>
        {
            Assert.False(string.IsNullOrWhiteSpace(result.Comment));
            Assert.Contains("has started", result.Comment);
            started = true;
        };
        await Executor.StartExecution(execution);
        Assert.True(started);
        Assert.True(finished);
    }


    [Fact]
    public async Task ExecuteWrongSyntax()
    {
        var execution = GetStartMessage("for (int i = 0; i < ; i++) System.Console.Write('+')");

        DispatcherApiMock.OnSetError = result => Assert.False(string.IsNullOrWhiteSpace(result.Comment));
        await Executor.StartExecution(execution);
    }
    
    [Fact]
    public async Task ExecuteEmpty()
    {
        var execution = GetStartMessage("");

        DispatcherApiMock.OnSetError = result => Assert.Contains("has empty source code", result.Comment);
        await Executor.StartExecution(execution);
    }
    
    [Fact]
    public async Task ExecuteBasicErrors()
    {
        var execution = GetStartMessage("throw new System.Exception(\"test-error\");");

        DispatcherApiMock.OnSetError = result =>
        {
            Assert.Contains("Execution finished", result.Comment);
            Assert.Contains("1 error", result.Comment);
            Assert.Contains("test-error", result.Data);
        };
        await Executor.StartExecution(execution);
    }
}