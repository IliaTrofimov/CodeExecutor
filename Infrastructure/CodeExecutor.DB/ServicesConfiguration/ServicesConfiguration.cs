using CodeExecutor.Common.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeExecutor.DB.ServicesConfiguration;

public static class ServicesConfiguration
{
    public static void AddDataBase(this IServiceCollection services, IConfigurationManager config, string path = "Database:PostgreSql")
    {
        var connection = config[path] 
                         ?? throw new ConfigurationException($"Missing '{path}' parameter at app settings.");
        
        services.AddDbContext<DataContext>(options =>
        {
            options.UseNpgsql(connection, x =>
                x.MigrationsAssembly("CodeExecutor.DB")
            );
        });
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}