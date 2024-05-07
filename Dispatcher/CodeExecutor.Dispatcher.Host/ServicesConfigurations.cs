using CodeExecutor.DB;
using CodeExecutor.DB.Repository;
using CodeExecutor.Common.Security;
using CodeExecutor.Dispatcher.Services.Implementations;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Dispatcher.Services.Utils;


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
        services.AddAutoMapper(o => o.AddProfile<AutoMapperProfile>());
    }

    public static void AddConfigs(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddAuthConfiguration(config);
        services.AddSingleton(new DbConfig(config.GetSection("Database")));
        services.AddSingleton(new Messaging.MessagingConfig(config.GetSection("RabbitMq")));
        services.AddSingleton<IConfiguration, ConfigurationManager>();
    }
}