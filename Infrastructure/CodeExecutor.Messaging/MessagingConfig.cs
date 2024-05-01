using CodeExecutor.Common;
using Microsoft.Extensions.Configuration;

namespace CodeExecutor.Messaging;

public class MessagingConfig 
{
    private string host;
    private string username;
    private string password;
    private int port;

    public string Host
    {
        get => host;
        private set => host = value ?? throw new ArgumentNullException(nameof(Host), "Missing host parameter");
    }
    public string Username  
    {
        get => username;
        private set => username = value ?? throw new ArgumentNullException(nameof(Username), "Missing username parameter");
    }
    public string Password  
    {
        get => password;
        private set => password = value ?? throw new ArgumentNullException(nameof(Password), "Missing password parameter");
    }
    public int Port 
    {
        get => port;
        private set => port = value;
    }
    
    
    public MessagingConfig(IConfiguration config) 
    {
        Host = config.GetValue("Host");
        Username = config.GetValue("Username");
        Password = config.GetValue("Password");
        Port = config.GetIntValue("Port");
    }
}