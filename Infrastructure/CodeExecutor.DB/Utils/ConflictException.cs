using System.Data.Common;

namespace CodeExecutor.DB.Utils;

public sealed class ConflictException : DbException
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
}