using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Auth.Contracts;


/// <summary>
/// Login response.
/// </summary>
public sealed class LoginResponse
{
    /// <summary>User Id.</summary>
    [Required]
    public long UserId { get; set; }
    
    /// <summary>Username.</summary>
    [Required]
    public string Username { get; set; } = null!;
    
    /// <summary>Auth token.</summary>
    [Required]
    public string Token { get; set; } = null!;
    
    /// <summary>Token expiration date.</summary>
    [Required]
    public DateTimeOffset ExpireDate { get; set; }
}