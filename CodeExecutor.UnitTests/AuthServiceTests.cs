using CodeExecutor.Auth.Contracts;
using CodeExecutor.Auth.Host.Services;
using CodeExecutor.Common.Models.Configs;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.DB.Exceptions;
using CodeExecutor.UnitTests.Mocks.Services;
using Xunit.Abstractions;


namespace CodeExecutor.UnitTests;

public class AuthServiceTests : DbTestBase
{
    protected readonly IAuthService AuthService;
    
    public AuthServiceTests(ITestOutputHelper output) : base(output, TestDbType.InMemory)
    {
        AuthService = new AuthService(UsersRepository, 
            new TestLogger<AuthService>(output),
            new TestAuthConfig());
    }

    private static LoginRequest GetLoginRequest(string username = "user", string password = "password")
    {
        return new LoginRequest { Username = username, Password = password };
    }

    private async Task<LoginResponse> CreateUserInternal(string? username = "user", string? password = "password")
    {
        var response = await AuthService.CreteUserAsync(new CreateUserRequest()
        {
            Username = username!,
            Password = password!,
            Email = $"{username}@email.com"
        });

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response.Username), "LoginResponse.Username cannot be null");
        Assert.False(string.IsNullOrWhiteSpace(response.Token), "LoginResponse.Token cannot be null");
        Assert.NotEqual(0, response.UserId);

        return response;
    }
    

    [Fact]
    public async Task CreateUserDefault()
    {
        await CreateUserInternal("CreateUserDefault");
    }
    
    [Fact]
    public async Task CreateUserExisting()
    {
        await CreateUserInternal("CreateUserExisting");
        await Assert.ThrowsAsync<ConflictException>(async () => 
            await CreateUserInternal("CreateUserExisting"));
    }
    
    [Fact]
    public async Task CreateUserBadRequest()
    {
        var ex = await Assert.ThrowsAsync<BadRequestException>(async () => 
            await CreateUserInternal(null, "null"));

        Assert.Contains("Username is required", ex.Message);
        
        ex = await Assert.ThrowsAsync<BadRequestException>(async () => 
            await CreateUserInternal("username", null));

        Assert.Contains("Password is required", ex.Message);
    }
    
    
    [Fact]
    public async Task LoginDefault()
    {
        var expected = await CreateUserInternal("LoginDefault");
        var loginResponse = await AuthService.LoginAsync(GetLoginRequest("LoginDefault"));
        
        Assert.NotNull(loginResponse);
        Assert.Equal(expected.UserId, loginResponse.UserId);
        Assert.Equal(expected.Username, loginResponse.Username);
        Assert.False(string.IsNullOrWhiteSpace(loginResponse.Username), "LoginResponse.Username cannot be null");
    }
    
    [Fact]
    public async Task LoginBadRequest()
    {
        var ex = await Assert.ThrowsAsync<BadRequestException>(async () => 
            await AuthService.LoginAsync(GetLoginRequest(null, "null")));
        Assert.Contains("Username is required", ex.Message);
        
        ex = await Assert.ThrowsAsync<BadRequestException>(async () => 
            await AuthService.LoginAsync(GetLoginRequest("username", null)));
        Assert.Contains("Password is required", ex.Message);
    }
    
    [Fact]
    public async Task LoginWrongPassword()
    {
        await CreateUserInternal("LoginWrongPassword", "password");
        
        var ex = await Assert.ThrowsAsync<UnauthorizedException>(async () => 
            await AuthService.LoginAsync(GetLoginRequest("x", "password")));
        Assert.Contains("Wrong password or username", ex.Message);
        
        ex = await Assert.ThrowsAsync<UnauthorizedException>(async () => 
            await AuthService.LoginAsync(GetLoginRequest("username", "x")));
        Assert.Contains("Wrong password or username", ex.Message);
    }
    
    
    [Fact]
    public async Task GeUserDefault()
    {
        var expected = await CreateUserInternal("GeUserDefault", "password");
        var actual = await AuthService.GetUserAsync(expected.UserId);
        Assert.NotNull(actual);
        Assert.Equal(expected.UserId, actual.UserId);
        Assert.Equal(expected.Username, actual.Username);
    }
    
    [Fact]
    public async Task GeUserNotExists()
    {
        var actual = await AuthService.GetUserAsync(100);
        Assert.Null(actual);

    }
}


internal class TestAuthConfig : IAuthConfig
{
    public string Key => "key_key_key_key_key_key_key_1234567890";
    public string Issuer => nameof(Issuer);
    public string Audience  => nameof(Audience);
    public int TokenLifespanMinutes => int.MaxValue;
}