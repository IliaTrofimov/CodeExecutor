namespace CodeExecutor.Messaging.Abstractions;

/// <summary>Base message queue client configuration.</summary>
public interface IMessagingConfig
{
    string Host { get; }
    string Username { get; }
    string Password { get; }
    int Port { get; }
}