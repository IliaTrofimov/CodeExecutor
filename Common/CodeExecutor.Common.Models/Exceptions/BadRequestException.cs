using System.Net;

namespace CodeExecutor.Common.Models.Exceptions;


/// <summary>
/// Request object has invalid data or format.
/// </summary>
public sealed class BadRequestException : ApiException
{
    public BadRequestException(string message, Exception? innerException = null)
        : base(message, HttpStatusCode.BadRequest, innerException) 
    {}
	    
    public BadRequestException(Exception? innerException = null)
        : base("Bad request.", HttpStatusCode.BadRequest, innerException) 
    {}
}