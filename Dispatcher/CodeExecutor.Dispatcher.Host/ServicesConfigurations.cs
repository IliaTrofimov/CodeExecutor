using System.Reflection;
using CodeExecutor.Common.Security;
using CodeExecutor.DB.ServicesConfiguration;
using CodeExecutor.Dispatcher.Host.Services.Implementations;
using CodeExecutor.Dispatcher.Host.Services.Interfaces;

namespace CodeExecutor.Dispatcher.Host;

public static class ServicesConfigurations
{
    public static void AddServices(this IServiceCollection services, ConfigurationManager config)
    {
        services.AddScoped<DbRepository.ICodeExecutionsExplorerRepository, DbRepository.CodeExecutionRepository>();
        services.AddScoped<DbRepository.ICodeExecutionsEditorRepository, DbRepository.CodeExecutionRepository>();
        services.AddScoped<DbRepository.ILanguagesRepository, DbRepository.LanguagesRepository>();
        
        services.AddScoped<IProgrammingLanguagesService, ProgrammingLanguagesService>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionDispatcher, CodeExecutionDispatcher>();
        services.AddScoped<ICodeExecutionMessaging, CodeExecutionMq>();
        services.AddScoped<ICodeExecutionExplorer, CodeExecutionExplorer>();
        
        services.AddHealthChecks().AddCheck<HealthCheckService>("DefaultHealthCheck");
        
        var tickerStr = config.GetSection("Extra")["Ticker"];
        if (tickerStr is not null)
            services.AddHostedService<TickerWorker>();
        
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