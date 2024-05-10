using Microsoft.Extensions.Configuration;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


namespace BaseCSharpExecutor.Api;

public class DispatcherApiConfig : IDispatcherApiConfig
{
    private readonly string dispatcherUrl;

    public string DispatcherUrl
    {
        get => dispatcherUrl;
        private init
        {
             dispatcherUrl = value.Trim().TrimEnd('/')
                             ?? throw new ArgumentNullException(nameof(DispatcherUrl), 
                                 "Missing DispatcherUrl parameter");
            try
            {
                new Uri(dispatcherUrl);
            }
            catch 
            {
                throw new ArgumentException("DispatcherUrl parameter must be a valid url",
                    nameof(DispatcherUrl));
            }
        }
           
    }

    public DispatcherApiConfig(IConfiguration config) { DispatcherUrl = config["DispatcherUrl"]!; }
}