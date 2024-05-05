using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.DB.Abstractions.Models;

/// <summary>Application user.</summary>
public class User : BaseEntity<long>
{
    public User()
    {
        CreatedAt = DateTime.Now;
    }
    
    /// <summary>User's login.</summary>
    [Required]
    [StringLength(64)]
    public string Username { get; set; } = null!;
    
    /// <summary>User's email.</summary>
    [StringLength(128)]
    public string? Email { get; set; }
    
    /// <summary>User's password hash.</summary>
    [Required]
    [StringLength(128)]
    public byte[] PasswordHash { get; set; } = null!;
     
    /// <summary>Is user an administrator.</summary>
    [Required] public bool IsSuperUser { get; set; }
    
    /// <summary>User account creation date.</summary>
    [Required] public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>Is user deactivated.</summary>
    [Required] public bool IsDeactivated { get; set; }

    /// <summary>User deactivation date.</summary>
    public DateTimeOffset? DeactivationDate { get; set; }
    
    
    public override string ToString() => IsSuperUser
        ? $"SuperUser(Guid={Id}, Username='{Username}')"
        : $"User(Guid={Id}, Username='{Username}')";
}