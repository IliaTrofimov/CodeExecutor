using CodeExecutor.Common.Health;
using CodeExecutor.Common.Security;
using CodeExecutor.DB;
using CodeExecutor.DB.Repository;
using CodeExecutor.Dispatcher.Services.Implementations;
using CodeExecutor.Dispatcher.Services.Interfaces;
using CodeExecutor.Dispatcher.Services.Utils;
using CodeExecutor.Messaging;


namespace CodeExecutor.Dispatcher.Host;

public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<DbRepository.ICodeExecutionsExplorerRepository, CodeExecutionRepository>();
        services.AddScoped<DbRepository.ICodeExecutionsEditorRepository, CodeExecutionRepository>();
        services.AddScoped<DbRepository.ILanguagesRepository, LanguagesRepository>();

        services.AddScoped<IProgrammingLanguagesService, ProgrammingLanguagesService>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionMessaging, CodeExecutionMq>();
        services.AddScoped<ICodeExecutionExplorer, CodeExecutionExplorer>();

        services.AddHealthChecks()
            .AddCheck<HealthCheckService>("DefaultHealthCheck")
            .AddCheck<PingCheckService>("Ping");
        
        services.AddAutoMapper(o => o.AddProfile<AutoMapperProfile>());
    }

    public static void AddConfigs(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthConfiguration(config);
        services.AddSingleton(new DbConfig(config.GetSection("Database")));
        services.AddSingleton(new MessagingConfig(config.GetSection("RabbitMq")));
        services.AddSingleton<IConfiguration, ConfigurationManager>();
    }
}