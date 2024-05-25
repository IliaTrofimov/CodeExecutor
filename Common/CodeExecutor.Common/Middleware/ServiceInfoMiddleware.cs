using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace CodeExecutor.Common.Middleware;

public class ServiceInfoMiddlewareOptions
{
    public string ServiceName { get; set; }
    public string Environment { get; set; }
    public string ServiceInstance { get; set; }
}

/// <summary>
/// Middleware adds serviceName and serviceVersion headers into each response.
/// </summary>
public class ServiceInfoMiddleware
{
    private readonly RequestDelegate next;
    private readonly string serviceName;
    private readonly string environment;
    private readonly string serviceInstance;


    public ServiceInfoMiddleware(RequestDelegate next, WebApplicationBuilder builder) 
    {
        this.next = next;
        this.serviceName = builder.Environment.ApplicationName;
        this.environment = builder.Environment.EnvironmentName;
        this.serviceInstance = builder.Configuration.GetValue<string>("ServiceInstance") ?? "default";
    }
  
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Append("Service-Name", serviceName);
        context.Response.Headers.Append("Service-Environment", environment);
        context.Response.Headers.Append("Service-Instance", serviceInstance);
        await next.Invoke(context);
    }
}