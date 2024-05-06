namespace CodeExecutor.Messaging.Abstractions;


/// <summary>
/// Message queue data receiver configuration.
/// </summary>
public interface IMessageReceiverConfig : IMessagingConfig
{
    string Queue { get; }
    string Exchange { get; }
}