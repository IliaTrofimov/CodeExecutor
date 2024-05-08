using System.ComponentModel.DataAnnotations;

namespace CodeExecutor.Auth.Contracts;


/// <summary>
/// User creation request.
/// </summary>
public sealed class CreateUserRequest
{
    /// <summary>Username.</summary>
    [Required]
    public string Username { get; set; } = null!;
    
    /// <summary>Password.</summary>
    [Required]
    public string Password { get; set; } = null!;
    
    /// <summary>User's email.</summary>
    public string? Email { get; set; }
}