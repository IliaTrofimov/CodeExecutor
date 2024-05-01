using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CodeExecutor.Common.Middleware;

/// <summary>Logging middleware.</summary>
public class DefaultHttpLogger
{
    private readonly RequestDelegate next;

    public DefaultHttpLogger(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext httpContext, ILogger<DefaultHttpLogger> logger, IWebHostEnvironment environment)
    {
        var requestLifeTime = await CallNext(httpContext);
        
        logger.Log(
            LogLevel.Information,
            "[{status}] {method} {url}{query}; Elapsed: {elapsed:F2} ms; Env: {env}",
            httpContext.Response?.StatusCode ?? 0,
            httpContext.Request.Method,
            httpContext.Request.Scheme + "://" + httpContext.Request.Host + httpContext.Request.Path,
            httpContext.Request.QueryString,
            requestLifeTime,
            environment.EnvironmentName);
    }

    private async Task<double> CallNext(HttpContext httpContext)
    {
        var timer = new Stopwatch();
        timer.Start();
        await next(httpContext);
        timer.Stop();

        return timer.Elapsed.TotalMilliseconds;
    }
}