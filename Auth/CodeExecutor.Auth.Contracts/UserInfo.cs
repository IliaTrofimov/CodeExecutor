using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Auth.Contracts;


/// <summary>
/// Information about user.
/// </summary>
public sealed class UserInfo
{
    /// <summary>Username.</summary>
    [Required]
    public string Username { get; set; } = null!;
    
    /// <summary>User's Id.</summary>
    [Required]
    public long UserId { get; set; }
    
    /// <summary>Is super user.</summary>
    [Required]
    public bool IsSuperUser { get; set; }
    
    /// <summary>Account creation date.</summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>User's email.</summary>
    public string? Email { get; set; } 
    
    /// <summary>Guid for anonymous user.</summary>
    public string? AnonymousToken { get; set; }
}