using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CodeExecutor.Auth.Contracts;
using CodeExecutor.Common.Models.Configs;
using CodeExecutor.Common.Models.Exceptions;
using CodeExecutor.DB.Abstractions.Models;
using CodeExecutor.DB.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace CodeExecutor.Auth.Host.Services;

public sealed class AuthService : IAuthService, IDisposable
{
    private readonly JwtSecurityTokenHandler tokenHandler;
    private readonly IUsersRepository usersRepository;
    private readonly ILogger<AuthService> logger;

    private readonly SHA512 sha512;

    private readonly string issuer;
    private readonly string audience;
    private readonly SymmetricSecurityKey key;
    private readonly int tokenLifeSpanMinutes;


    public AuthService(IUsersRepository usersRepository,
                       ILogger<AuthService> logger,
                       IAuthConfig config)
    {
        this.usersRepository = usersRepository;
        this.logger = logger;
        tokenHandler = new JwtSecurityTokenHandler();
        sha512 = SHA512.Create();

        issuer = config.Issuer;
        audience = config.Audience;
        tokenLifeSpanMinutes = config.TokenLifespanMinutes;
        key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.Key));
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new BadRequestException("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Password is required");

        Task<User?> getUser = usersRepository
            .Query()
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        var hash = GetPasswordHash(request.Password);
        var user = await getUser;

        if (user is null || !CompareHashes(user.PasswordHash, hash))
            throw new UnauthorizedException("Wrong password or username");

        return new LoginResponse
        {
            Username = request.Username,
            UserId = user.Id,
            Token = GetToken(user, out var expires),
            ExpireDate = expires
        };
    }

    public async Task<LoginResponse> CreteUserAsync(CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new BadRequestException("Username is required");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new BadRequestException("Password is required");

        var hash = GetPasswordHash(request.Password);
        var user = await usersRepository.Create(new User
        {
            PasswordHash = hash,
            Username = request.Username,
            Email = request.Email
        });

        await usersRepository.SaveAsync();

        return new LoginResponse
        {
            Username = request.Username,
            UserId = user.Id,
            Token = GetToken(user, out var expires),
            ExpireDate = expires
        };
    }

    public async Task<UserInfo?> GetUserAsync(long userId)
    {
        var user = await usersRepository.GetAsync(userId);
        if (user is null)
            return null;

        return new UserInfo
        {
            UserId = userId,
            Username = user.Username,
            Email = user.Email,
            IsSuperUser = user.IsSuperUser,
            CreatedAt = user.CreatedAt
        };
    }


    private string GetToken(User user, out DateTimeOffset expires)
    {
        expires = new DateTimeOffset(DateTime.Now.AddMinutes(tokenLifeSpanMinutes)).DateTime;
        var token = new JwtSecurityToken(
            issuer,
            audience,
            notBefore: new DateTimeOffset(DateTime.Now).DateTime,
            expires: expires.DateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), "long"),
                new Claim(ClaimTypes.Name, user.Username, "string"),
                new Claim("IsSuperUser", user.IsSuperUser.ToString(), "bool")
            }
        );

        return tokenHandler.WriteToken(token);
    }

    private byte[] GetPasswordHash(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        return sha512.ComputeHash(bytes);
    }

    private static bool CompareHashes(byte[] expected, byte[] actual)
    {
        if (expected.Length != actual.Length)
            return false;

        return !expected.Where((t, i) => actual[i] != t).Any();
    }


    public void Dispose() { sha512.Dispose(); }
}