using Microsoft.Extensions.Configuration;

namespace CodeExecutor.Common.Security;

public class AuthConfig
{
    private string key;
    private string issuer;
    private string audience;
    private int tokenLifespan;

    public string Key
    {
        get => key;
        set => key = value ?? throw new ArgumentNullException(nameof(Key), "Missing key parameter");
    }
    public string Issuer  
    {
        get => issuer;
        set => issuer = value ?? throw new ArgumentNullException(nameof(Issuer), "Missing issuer parameter");
    }
    public string Audience  
    {
        get => audience;
        set => audience = value ?? throw new ArgumentNullException(nameof(Audience), "Missing audience parameter");
    }
    public int TokenLifespanMinutes
    {
        get => tokenLifespan;
        set => tokenLifespan = value;
    }


    public AuthConfig(IConfiguration config) 
    {
        Key = config.GetValue("Key");
        Issuer = config.GetValue("Issuer");
        Audience = config.GetValue("Audience");
        TokenLifespanMinutes = config.GetIntValue("TokenLifespanMinutes");
    }
}