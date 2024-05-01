using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CodeExecutor.Common.Logging;

/// <summary>File provider for logger.</summary>
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly IWebHostEnvironment environment;
    private readonly string logFileName;
    
    public FileLoggerProvider(IWebHostEnvironment environment, string? logFileName = "log.txt")
    {
        this.environment = environment;
        this.logFileName = logFileName ?? "log.txt";
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(environment, logFileName);
    }

    private class FileLogger : ILogger, IDisposable
    {
        private static object _obj = new object();
        private IWebHostEnvironment environment;
        private readonly string logFileName;


        public FileLogger(IWebHostEnvironment environment, string logFileName)
        {
            this.environment = environment;
            this.logFileName = logFileName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel) => true;


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (environment.IsDevelopment() || logLevel is LogLevel.Information or LogLevel.Error)
            {
                lock (_obj)
                {
                    File.AppendAllLines(logFileName, new[] {$"{formatter(state, exception)}\n"});
                }
            }
        }

        public void Dispose()
        {
        }
    }
}