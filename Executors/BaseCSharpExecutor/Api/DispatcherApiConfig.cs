using Microsoft.Extensions.Configuration;

namespace BaseCSharpExecutor.Api;

public class DispatcherApiConfig
{
    private string dispatcherUrl;

    public string DispatcherUrl
    {
        get => dispatcherUrl;
        private set => dispatcherUrl = value?.Trim().TrimEnd('/')
                                       ?? throw new ArgumentNullException(nameof(DispatcherUrl), "Missing DispatcherUrl parameter");
    }

    public DispatcherApiConfig(IConfiguration config)
    {
        DispatcherUrl = config["DispatcherUrl"];
    }
}