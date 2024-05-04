using CodeExecutor.Common;
using Microsoft.Extensions.Configuration;

namespace CodeExecutor.Messaging;

public class MessageReceiverConfig : MessagingConfig, IMessageReceiverConfig
{
    private string queue;
    private string exchange;

    public string Queue  
    {
        get => queue;
        private init => queue = value ?? throw new ArgumentNullException(nameof(Queue), "Missing queue parameter");
    }
    
    public string Exchange  
    {
        get => exchange;
        private init => exchange = value ?? throw new ArgumentNullException(nameof(Exchange), "Missing exchange parameter");
    }


    public MessageReceiverConfig(IConfiguration config) : base(config)
    {
        Queue = config.GetValue("ReceiveQueue");
        Exchange = config.GetValue("ReceiveExchange");
    }
}