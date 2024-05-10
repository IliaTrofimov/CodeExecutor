using CodeExecutor.Auth.Host.Services;
using CodeExecutor.Common.Health;
using CodeExecutor.Common.Security;
using CodeExecutor.DB.Abstractions.Repository;
using CodeExecutor.DB.Repository;


namespace CodeExecutor.Auth.Host;

public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddHealthChecks()
            .AddCheck<HealthCheckService>("DefaultHealthCheck")
            .AddCheck<PingCheckService>("PingHealthCheck", timeout: new TimeSpan(0, 0, 1));
    }

    public static void AddConfigs(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthConfiguration(config);
    }
}