namespace CodeExecutor.Messaging.Abstractions.Services;

/// <summary>
/// Basic interface for sending messages to MQ.
/// </summary>
public interface IMessageSender
{
    /// <summary> Send given object to queue.</summary>
    public Task SendAsync<TMessage>(TMessage message, string queue, string exchange = "", byte priority = 0)
        where TMessage: class;
}