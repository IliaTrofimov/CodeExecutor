using System.Collections.ObjectModel;
using System.Diagnostics;
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
        
        ReadOnlyCollection<ApiFault> faults = ApiFault.Create(ex);
        var response = JsonSerializer.Serialize(faults);

        AddErrorEvent(ex, response);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.Code;

        return context.Response.WriteAsync(response);
    }

    private static void AddErrorEvent(ApiException exception, string faultsString)
    {
        if (Activity.Current is null) return;

        var tags = new ActivityTagsCollection
        {
            new("exception.Type", exception.GetType().Name),
            new("exception.Message", exception.Message),
            new("exception.Trace", exception.StackTrace)
        };
        
        Activity.Current.AddEvent(new ActivityEvent("Unhandled exception", tags: tags));
        Activity.Current.AddBaggage("Faults", faultsString);
    }
}