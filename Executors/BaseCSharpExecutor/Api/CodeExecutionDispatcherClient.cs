using System.Net.Http.Json;
using CodeExecutor.Dispatcher.Contracts;


namespace BaseCSharpExecutor.Api;

public class CodeExecutionDispatcherClient : ICodeExecutionDispatcherClient
{
    private readonly HttpClient httpClient;
    private readonly string dispatcherUrl;

    
    public CodeExecutionDispatcherClient(HttpClient httpClient, DispatcherApiConfig config)
    {
        this.httpClient = httpClient;
        this.dispatcherUrl = config.DispatcherUrl;
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
}