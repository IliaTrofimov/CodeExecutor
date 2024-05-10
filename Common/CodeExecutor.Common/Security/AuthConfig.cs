using CodeExecutor.Common.Models.Configs;
using Microsoft.Extensions.Configuration;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CodeExecutor.Common.Security;

public class AuthConfig : IAuthConfig
{
    private readonly string key;
    private readonly string issuer;
    private readonly string audience;

    public string Key
    {
        get => key;
        private init => key = value ?? throw new ArgumentNullException(nameof(Key), "Missing key parameter");
    }

    public string Issuer
    {
        get => issuer;
        private init => issuer = value ?? throw new ArgumentNullException(nameof(Issuer), "Missing issuer parameter");
    }

    public string Audience
    {
        get => audience;
        private init =>
            audience = value ?? throw new ArgumentNullException(nameof(Audience), "Missing audience parameter");
    }

    public int TokenLifespanMinutes { get; }


    public AuthConfig(IConfiguration config)
    {
        Key = config.GetValue("Key");
        Issuer = config.GetValue("Issuer");
        Audience = config.GetValue("Audience");
        TokenLifespanMinutes = config.GetIntValue("TokenLifespanMinutes");
    }
}