using CodeExecutor.Auth.Host.Services;
using CodeExecutor.Common.Security;
using CodeExecutor.DB.Abstractions.Repository;
using CodeExecutor.DB.Repository;

namespace CodeExecutor.Auth.Host;

public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddHealthChecks().AddCheck<HealthCheckService>("DefaultHealthCheck");
    }

    public static void AddConfigs(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAuthConfiguration(config);
    }
}