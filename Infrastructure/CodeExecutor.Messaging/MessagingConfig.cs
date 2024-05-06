using CodeExecutor.Common.Models.Configs;
using CodeExecutor.Messaging.Abstractions;
using Microsoft.Extensions.Configuration;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CodeExecutor.Messaging;

public class MessagingConfig : IMessagingConfig
{
    private string host = null!;
    private string username = null!;
    private string password = null!;

    public string Host
    {
        get => host;
        private init => host = value ?? throw new ArgumentNullException(nameof(Host), "Missing host parameter");
    }
    public string Username  
    {
        get => username;
        private init => username = value ?? throw new ArgumentNullException(nameof(Username), "Missing username parameter");
    }
    public string Password  
    {
        get => password;
        private init => password = value ?? throw new ArgumentNullException(nameof(Password), "Missing password parameter");
    }
    public int Port { get; }


    public MessagingConfig(IConfiguration config) 
    {
        Host = config.GetValue("Host");
        Username = config.GetValue("Username");
        Password = config.GetValue("Password");
        Port = config.GetIntValue("Port");
    }
}