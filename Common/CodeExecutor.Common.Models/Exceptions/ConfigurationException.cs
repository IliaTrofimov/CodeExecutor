namespace CodeExecutor.Common.Models.Exceptions;

public sealed class ConfigurationException : ApiException
{
    public ConfigurationException(string message, Exception? innerException = null)
        : base(message, 500, innerException)
    {}
    
    public ConfigurationException(string param, string environment, Exception? innerException = null)
        : this($"Missing '{param}' parameter at {environment} environment settings.",  innerException)
    {}
    
    public ConfigurationException(string param, Type expectedType, string environment, Exception? innerException = null)
        : this($"Parameter '{param}' at {environment} environment settings must be {expectedType}.", innerException)
    {}
}