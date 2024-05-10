using System.Net;


namespace CodeExecutor.Common.Models.Exceptions;

/// <summary>Current user is unauthorized to perform action.</summary>
public sealed class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message, Exception? innerException = null)
        : base($"Unauthorized to execute this action. {message}", HttpStatusCode.Unauthorized, innerException)
    {
    }

    public UnauthorizedException(Exception? innerException = null)
        : base("Unauthorized to execute this action.", HttpStatusCode.Unauthorized, innerException)
    {
    }
}