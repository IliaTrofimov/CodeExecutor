using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CodeExecutor.Messaging.Services;

public abstract class BasicMessagingClient: IDisposable
{
    protected readonly IModel rabbitChannel;
    protected readonly IConnection rabbitConnection;
    protected readonly ILogger<BasicMessageSender> logger;
    
    protected BasicMessagingClient(IMessagingConfig config, ILogger<BasicMessageSender> logger)
    {
        this.logger = logger;
        this.logger.LogDebug("Connecting to RabbitMQ");

        var rabbitFactory = new ConnectionFactory()
        {
            HostName = config.Host,
            UserName = config.Username,
            Password = config.Password,
            Port = config.Port,
            AutomaticRecoveryEnabled = true
        };

        try
        {
            rabbitConnection = rabbitFactory.CreateConnection();
            rabbitChannel = rabbitConnection.CreateModel();
        }
        catch (Exception ex)
        {
            this.logger.LogError("Connection to RabbitMQ FAILED:\n{error}", ex.Message);
            throw new InfrastructureException("Unable to connect to RabbitMQ");
        }
        this.logger.LogDebug("Successfully connected to RabbitMQ");
    }
    
    public void Dispose()
    {
        rabbitConnection.Dispose();
        rabbitChannel.Dispose();
    }
}