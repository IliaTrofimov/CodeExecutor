using Microsoft.Extensions.Logging;
using Xunit.Abstractions;


namespace CodeExecutor.UnitTests.Mocks.Services;

public class TestLogger<TService>(ITestOutputHelper output) : TestLogger(output, typeof(TService).Name),
                                                              ILogger<TService>
    where TService : class;


public class TestLogger(ITestOutputHelper output, string serviceName = "") : ILogger
{
    private readonly string preffix = string.IsNullOrWhiteSpace(serviceName)
        ? ":\n     "
        : $" {serviceName}:\n     ";


    public IDisposable BeginScope<TState>(TState state) where TState : notnull => new NoopDisposable();

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                            Func<TState, Exception?, string> formatter)
    {
        output.WriteLine(
            $"{DateTime.Now:HH:mm:ss.ff} [{logLevel.ToString().ToUpper()}]{preffix}{formatter(state, exception)}");

        if (logLevel is LogLevel.Critical or LogLevel.Error)
            output.WriteLine("");
    }


    private class NoopDisposable : IDisposable
    {
        public void Dispose() { }
    }
}