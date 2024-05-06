using System.Reflection;
using CodeExecutor.Common.Security;
using CodeExecutor.DB;
using CodeExecutor.DB.Repository;
using CodeExecutor.Dispatcher.Services.Implementations;
using CodeExecutor.Dispatcher.Services.Interfaces;

namespace CodeExecutor.Dispatcher.Host;

public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddScoped<DbRepository.ICodeExecutionsExplorerRepository, CodeExecutionRepository>();
        services.AddScoped<DbRepository.ICodeExecutionsEditorRepository, CodeExecutionRepository>();
        services.AddScoped<DbRepository.ILanguagesRepository, LanguagesRepository>();
        
        services.AddScoped<IProgrammingLanguagesService, ProgrammingLanguagesService>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionMessaging, CodeExecutionMq>();
        services.AddScoped<ICodeExecutionExplorer, CodeExecutionExplorer>();
        
        services.AddHealthChecks().AddCheck<HealthCheckService>("DefaultHealthCheck");
        services.AddAutoMapper(Assembly.GetAssembly(typeof(Program)));
    }

    public static void AddConfigs(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAuthConfiguration(config);
        services.AddSingleton(new DbConfig(config.GetSection("Database")));
        services.AddSingleton(new Messaging.MessagingConfig(config.GetSection("RabbitMq")));
        services.AddSingleton<IConfiguration, ConfigurationManager>();
    }
}