using System.Text;
using System.Text.Json;
using CodeExecutor.Messaging.Abstractions;
using CodeExecutor.Messaging.Abstractions.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace CodeExecutor.Messaging.Services;

public abstract class BasicMessageReceiver<TMessage> : BasicMessagingClient, IMessageReceiver<TMessage>
    where TMessage : class
{
    protected readonly EventingBasicConsumer rabbitConsumer;
    protected readonly string queue;
    protected readonly string exchange;


    protected BasicMessageReceiver(IMessageReceiverConfig config, ILogger<BasicMessageSender> logger)
        : base(config, logger)
    {
        queue = config.Queue;
        exchange = config.Exchange;
        rabbitConsumer = new EventingBasicConsumer(rabbitChannel);
        rabbitConsumer.Received += HandleRaw;
    }

    public abstract void HandleMessage(TMessage message);


    public void StartReceive()
    {
        logger.LogDebug("MessageReceiver {messageReceiver} is starting consuming...", GetType());

        rabbitChannel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
        rabbitChannel.QueueDeclare(queue, true, false, false);
        rabbitChannel.QueueBind(queue, exchange, queue);
        rabbitChannel.BasicConsume(queue, true, rabbitConsumer);

        logger.LogInformation("MessageReceiver {messageReceiver} is consuming", GetType());
    }

    public void StopReceive() { rabbitConsumer.HandleModelShutdown(rabbitChannel, null); }


    private void HandleRaw(object? sender, BasicDeliverEventArgs e)
    {
        var text = Encoding.UTF8.GetString(e.Body.ToArray());
        try
        {
            var message = JsonSerializer.Deserialize<TMessage>(text);

            if (message is null)
                logger.LogError("Cannot deserialize message {messageType}: Result object is null",
                    typeof(TMessage));
            else
                HandleMessage(message);
        }
        catch (JsonException ex)
        {
            logger.LogError("Cannot deserialize message {messageType}: {error}",
                typeof(TMessage), ex);
        }
        catch (Exception ex)
        {
            logger.LogError("Error processing message {messageType} ({errorType}): {error}",
                typeof(TMessage), ex.GetType(), ex);
        }
    }
}