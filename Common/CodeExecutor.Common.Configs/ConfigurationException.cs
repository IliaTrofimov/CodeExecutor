namespace CodeExecutor.Common.Configs;

public sealed class ConfigurationException : Exception
{
    public ConfigurationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {}
    
    public ConfigurationException(string param, string environment, Exception? innerException = null)
        : this($"Missing '{param}' parameter at {environment} environment settings.",  innerException)
    {}
    
    public ConfigurationException(string param, Type expectedType, string environment, Exception? innerException = null)
        : base($"Parameter '{param}' at {environment} environment settings must be {expectedType}.", innerException)
    {}
}