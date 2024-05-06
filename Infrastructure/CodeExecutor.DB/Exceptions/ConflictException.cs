using System.Data.Common;
using CodeExecutor.Common.Models.Exceptions;

namespace CodeExecutor.DB.Exceptions;

public sealed class ConflictException : DbException, IApiException
{
    public ConflictException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        Data.Add("Code", 409);
    }
    
    public ConflictException(string itemName, string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        Data.Add("ItemName", itemName);
        Data.Add("Code", 409);
    }

    public string ErrorType => nameof(ConflictException);
    public int Code => 409;
}