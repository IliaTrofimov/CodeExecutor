using CodeExecutor.Auth.Contracts;


namespace CodeExecutor.Auth.Host.Services;

/// <summary>Authorization and authentication functions.</summary>
public interface IAuthService
{
    /// <summary>Login user.</summary>
    public Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>Create new user.</summary>
    public Task<LoginResponse> CreteUserAsync(CreateUserRequest request);

    /// <summary>Get user info by Id.</summary>
    public Task<UserInfo?> GetUserAsync(long userId);
}