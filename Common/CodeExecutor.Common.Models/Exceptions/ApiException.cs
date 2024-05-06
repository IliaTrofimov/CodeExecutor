using System.Collections;
using System.Net;

namespace CodeExecutor.Common.Models.Exceptions;

/// <summary>
/// Base exception class for all WebApi errors.
/// </summary>
public interface IApiException
{
    /// <summary>Name of this exception.</summary>
    string ErrorType { get; }

    /// <summary>Integer code of this exception.</summary>
    int Code { get; }

    IDictionary Data { get; }
    string Message { get; }
    string? StackTrace { get; }
}

/// <summary>
/// Base exception class for all WebApi errors.
/// </summary>
public class ApiException : Exception, IApiException
{
    /// <summary>Name of this exception.</summary>
    public string ErrorType { get; private set; }
    
    /// <summary>Integer code of this exception.</summary>
    public int Code { get; protected set; }
    
    
    public ApiException(string message, int code = 500, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorType = GetType().Name;
        Code = code;
    }
		
    public ApiException(int code = 500, Exception? innerException = null) 
        : this("Internal server error.", code, innerException) {}
		
    public ApiException(HttpStatusCode code, Exception? innerException = null) 
        : this((int)code, innerException) {}
		
    public ApiException(string message, HttpStatusCode code, Exception? innerException = null) 
        : this(message, (int)code, innerException) {}


    /// <summary>Create ApiException object from Exception.</summary>
    public static ApiException FromBase(Exception exception)
    {
        if (exception is ApiException apiException)
            return apiException;
        
        apiException = new ApiException($"Unhandled exception: {exception.Message}");
        apiException.ErrorType = exception.GetType().Name;
        
        foreach (var key in exception.Data)
            apiException.Data.Add(key, exception.Data[key]);

        return apiException;
    }
}