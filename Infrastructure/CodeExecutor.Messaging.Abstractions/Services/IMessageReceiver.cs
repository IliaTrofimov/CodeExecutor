namespace CodeExecutor.Messaging.Abstractions.Services;

/// <summary>Basic interface for receiving messages from queue.</summary>
public interface IMessageReceiver<in TMessage> where TMessage : class
{
    /// <summary>Start listening.</summary>
    public void StartReceive();

    /// <summary>Stop listening.</summary>
    public void StopReceive();

    /// <summary>Handle message.</summary>
    public void HandleMessage(TMessage message);
}