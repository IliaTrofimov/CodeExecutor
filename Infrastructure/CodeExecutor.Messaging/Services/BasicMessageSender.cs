using System.Text;
using System.Text.Json;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Messaging.Abstractions.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;


namespace CodeExecutor.Messaging.Services;

public abstract class BasicMessageSender : BasicMessagingClient, IMessageSender
{
    protected BasicMessageSender(MessagingConfig config, ILogger<BasicMessageSender> logger) : base(config, logger) { }

    public async Task SendAsync<TMessage>(TMessage message, string queue, string exchange = "", byte priority = 0)
        where TMessage : class
    {
        logger.LogDebug("Sending message {messageType} to exchange='{exchange}' queue='{queue}'",
            typeof(TMessage), exchange, queue);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var properties = rabbitChannel.CreateBasicProperties();
        properties.Priority = priority;

        try
        {
            rabbitChannel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
            rabbitChannel.QueueBind(queue, exchange, queue);

            await Task.Run(() => rabbitChannel.BasicPublish(exchange,
                queue,
                properties,
                body));

            logger.LogInformation("Message {messageType} was sent successfully", typeof(TMessage));
        }
        catch (Exception ex)
        {
            throw new InfrastructureException(ex.Message, ex);
        }
    }
}