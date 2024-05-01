using System.Collections.ObjectModel;
using CodeExecutor.Common.Models.Exceptions;

namespace CodeExecutor.Common.Models.Entities;

public sealed class ApiFault
{
    /// <summary>Name of this exception.</summary>
    public string ErrorType { get; private set; }
    
    /// <summary>Integer code of this exception.</summary>
    public int Code { get; private set; }
    
    public string Message { get; private set; }
    
    public Dictionary<string, string>? Data { get; private set; }


    private ApiFault(ApiException exception)
    {
        Code = exception.Code;
        Message = exception.Message;
        ErrorType = exception.ErrorType;
        
        if (exception.Data.Count > 0)
        {
            Data = new Dictionary<string, string>(exception.Data.Count);
            foreach (var key in exception.Data.Keys)
            {
                if (key is not null && exception.Data[key] is not null)
                    Data.Add(key.ToString(), exception.Data[key].ToString());
            }
        }
    }

    private ApiFault(Exception exception) : this(ApiException.FromBase(exception))
    {
    }
    
    public static ReadOnlyCollection<ApiFault> Create(ApiException apiException)
    {
        var faults = new List<ApiFault>();

        Exception? current = apiException.InnerException;
        faults.Add(new ApiFault(apiException));
        var depth = 4;
        
        while (depth > 0 && current is not null)
        {
            faults.Add(new ApiFault(current));
            depth--;
            current = current.InnerException;
        }
        return faults.AsReadOnly();
    }
    
    public static ReadOnlyCollection<ApiFault> Create(Exception exception)
    {
        var faults = new List<ApiFault>();

        Exception? current = exception;
        var depth = 5;
        
        while (depth > 0 && current is not null)
        {
            faults.Add(new ApiFault(current));
            depth--;
            current = current.InnerException;
        }
        return faults.AsReadOnly();
    }
}