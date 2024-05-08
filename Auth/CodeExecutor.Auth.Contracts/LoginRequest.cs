using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Auth.Contracts;


/// <summary>
/// Login request body.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>Username.</summary>
    [Required]
    public string Username { get; set; } = null!;
    
    /// <summary>Password.</summary>
    [Required]
    public string Password { get; set; } = null!;
}