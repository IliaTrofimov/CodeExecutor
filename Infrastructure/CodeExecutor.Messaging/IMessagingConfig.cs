namespace CodeExecutor.Messaging;

public interface IMessagingConfig
{
    string Host { get; }
    string Username { get; }
    string Password { get; }
    int Port { get; }
}