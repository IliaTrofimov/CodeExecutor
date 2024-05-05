namespace CodeExecutor.Messaging;

public interface IMessageReceiverConfig : IMessagingConfig
{
    string Queue { get; }
    string Exchange { get; }
}