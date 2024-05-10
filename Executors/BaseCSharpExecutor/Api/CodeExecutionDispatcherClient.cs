using System.Net.Http.Json;
using CodeExecutor.Dispatcher.Contracts;
using Microsoft.Extensions.Logging;


namespace BaseCSharpExecutor.Api;

public class CodeExecutionDispatcherClient : ICodeExecutionDispatcherClient
{
    private readonly HttpClient httpClient;
    private readonly string dispatcherUrl;
    private readonly ILogger<ICodeExecutionDispatcherClient> logger;

    public CodeExecutionDispatcherClient(HttpClient httpClient, IDispatcherApiConfig config,  ILogger<ICodeExecutionDispatcherClient> logger)
    {
        this.httpClient = httpClient;
        dispatcherUrl = config.DispatcherUrl;
        this.logger = logger;
    }


    public async Task SetResultAsync(CodeExecutionResult codeExecutionResult, string validationTag,
                                     CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch,
            $"{dispatcherUrl}/CodeExecutionsModification/SetResult");

        request.Headers.Add("ValidationTag", validationTag);
        request.Content = JsonContent.Create(codeExecutionResult);

        await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
    }

    public async Task<bool> TryPingAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Try ping CodeExecutionDispatcher");

        try
        {
            var response = await httpClient.GetAsync($"{dispatcherUrl}/ping", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("CodeExecutionDispatcher is available");
                return true;
            }
            
            logger.LogError("CodeExecutionDispatcher is unavailable:/n{error}",
                await response.Content.ReadAsStringAsync(cancellationToken));
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError("CodeExecutionDispatcher is unavailable. Cannot access '{dispatcherUrl}/ping':{error}",
                            dispatcherUrl, ex.ToString());
            return false;
        }
    }
}