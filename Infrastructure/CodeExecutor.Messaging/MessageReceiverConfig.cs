using CodeExecutor.Common;
using Microsoft.Extensions.Configuration;

namespace CodeExecutor.Messaging;

public class MessageReceiverConfig : MessagingConfig
{
    private string queue;
    private string exchange;

    public string Queue  
    {
        get => queue;
        private set => queue = value ?? throw new ArgumentNullException(nameof(Queue), "Missing queue parameter");
    }
    
    public string Exchange  
    {
        get => exchange;
        private set => exchange = value ?? throw new ArgumentNullException(nameof(Exchange), "Missing exchange parameter");
    }


    public MessageReceiverConfig(IConfiguration config) : base(config)
    {
        Queue = config.GetValue("ReceiveQueue");
        Exchange = config.GetValue("ReceiveExchange");
    }
}