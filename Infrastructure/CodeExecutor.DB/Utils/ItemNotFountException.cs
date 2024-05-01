using System.Data.Common;

namespace CodeExecutor.DB.Utils;

public sealed class ItemNotFountException : DbException
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
}