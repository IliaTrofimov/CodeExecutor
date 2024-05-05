namespace CodeExecutor.Common.Security;

public interface IAuthConfig
{
    string Key { get; }
    string Issuer { get; }
    string Audience { get; }
    int TokenLifespanMinutes { get; }
}