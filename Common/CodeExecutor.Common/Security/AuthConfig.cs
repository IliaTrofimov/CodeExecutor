using CodeExecutor.Common.Models.Configs;
using Microsoft.Extensions.Configuration;

namespace CodeExecutor.Common.Security;

public class AuthConfig : IAuthConfig
{
    private string key;
    private string issuer;
    private string audience;

    public string Key
    {
        get => key;
        private set => key = value ?? throw new ArgumentNullException(nameof(Key), "Missing key parameter");
    }
    public string Issuer  
    {
        get => issuer;
        private set => issuer = value ?? throw new ArgumentNullException(nameof(Issuer), "Missing issuer parameter");
    }
    public string Audience  
    {
        get => audience;
        private set => audience = value ?? throw new ArgumentNullException(nameof(Audience), "Missing audience parameter");
    }
    public int TokenLifespanMinutes { get; private set; }


    public AuthConfig(IConfiguration config) 
    {
        Key = config.GetValue("Key");
        Issuer = config.GetValue("Issuer");
        Audience = config.GetValue("Audience");
        TokenLifespanMinutes = config.GetIntValue("TokenLifespanMinutes");
    }
}