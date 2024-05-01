using System.Text.Json;
using CodeExecutor.Common.Models.Entities;
using CodeExecutor.Common.Models.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace CodeExecutor.Common.Middleware;

public class DefaultExceptionHandler
{
    private readonly RequestDelegate next;
    private readonly ILogger<DefaultExceptionHandler> logger;
    
    public DefaultExceptionHandler(RequestDelegate next, ILogger<DefaultExceptionHandler> logger)
    {
        this.logger = logger;
        this.next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ApiException ex)
        {
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ApiException.FromBase(ex));
        }
    }

    private Task HandleExceptionAsync(HttpContext context, ApiException ex)
    {
        logger.LogError("Unexpected server error ({exception}):\n{message}", ex.GetType().Name, ex.Message);
        var faults = ApiFault.Create(ex);
        var response = JsonSerializer.Serialize(faults);
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.Code;
        
        return context.Response.WriteAsync(response);
    }
}