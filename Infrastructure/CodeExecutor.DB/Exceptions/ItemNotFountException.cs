using System.Data.Common;
using CodeExecutor.Common.Models.Exceptions;


namespace CodeExecutor.DB.Exceptions;

public sealed class ItemNotFountException : DbException, IApiException
{
    public ItemNotFountException(string itemName, Exception? innerException = null)
        : base($"Cannot find {itemName} with", innerException)
    {
        Data.Add("ItemName", itemName);
        Data.Add("Code", 404);
    }

    public ItemNotFountException(string itemName, object key, Exception? innerException = null)
        : base($"Cannot find {itemName} with key '{key}'", innerException)
    {
        Data.Add("ItemName", itemName);
        Data.Add("Code", 404);
        Data.Add("Key", key);
    }

    public string ErrorType => nameof(ItemNotFountException);
    public int Code => 404;
}