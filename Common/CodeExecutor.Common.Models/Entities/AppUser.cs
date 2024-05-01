namespace CodeExecutor.Common.Models.Entities;


public sealed class AppUser
{
    public const long AnonymousUserId = -1;
    public const string AnonymousUserName = "__anonymous__";
    
    private long id = AnonymousUserId;
    private string username = AnonymousUserName;

    public long Id
    {
        get => id;
        set => id = IsAnonymous ? AnonymousUserId : value;
    }
    
    public string Username
    {
        get => username;
        set => username = IsAnonymous ? AnonymousUserName : value;
    }
    
    public bool IsSuperUser { get; set; }
    
    public bool IsAnonymous { get; set; }
    
    public Dictionary<string, string> Claims { get; set; } = new Dictionary<string, string>(0);
}