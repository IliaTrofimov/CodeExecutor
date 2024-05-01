namespace CodeExecutor.Common.Models.Exceptions;

public class InfrastructureException : ApiException
{
    public InfrastructureException(string reason, Exception? innerException = null)
        : base($"Cannot execute operation because of some infrastructure error: {reason}.", 500, innerException)
    {}
	    
    public InfrastructureException(Exception? innerException = null)
        : base("Cannot execute operation because of some infrastructure error.", 500, innerException)
    {}
}