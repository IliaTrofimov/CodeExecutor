using CodeExecutor.Common.Security;
using CodeExecutor.Auth.Host.Services;
using CodeExecutor.DB.Repository;
using CodeExecutor.DB.Abstractions.Repository;


namespace CodeExecutor.Auth.Host;


public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddHealthChecks().AddCheck<HealthCheckService>("DefaultHealthCheck");
    }

    public static void AddConfigs(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthConfiguration(config);
    }
}