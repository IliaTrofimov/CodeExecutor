using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

using CodeExecutor.Common.Models.Exceptions;
using Microsoft.Extensions.DependencyInjection;


namespace CodeExecutor.Common;

public static class ConfigurationExtensions
{
    public static TConfig Bind<TConfig>(this IConfiguration config, string path)
        where TConfig: class, new()
    {
        var section = config.GetSection(path);
        if (!section.Exists())
            throw new ConfigurationException($"Cannot bind configuration file to type {typeof(TConfig)}. Section '{path}' is missing.");

        var configObject = new TConfig();
        try
        {
            config.Bind(configObject);
        }
        catch (Exception ex)
        {
            throw new ConfigurationException($"Cannot bind configuration file to type {typeof(TConfig)}. {ex.Message}");
        }

        return configObject;
    }

    public static void AddConfigurationObject<TConfig>(this IServiceCollection services, IConfiguration config, string path)
        where TConfig: class, new()
    {
        var configObject = config.Bind<TConfig>(path);
        services.AddSingleton(configObject);
    }
    
    public static string GetValue(this IConfiguration config, string path)
    {
        return config[path] 
               ?? throw new ConfigurationException($"Missing '{path}' parameter at app settings.");
    }
    
    public static bool TryGetValue(this IConfiguration config, string path, [NotNullWhen(true)] out string? value)
    {
        value = config[path];
        return value is not null;
    }
    
    public static int GetIntValue(this IConfiguration config, string path)
    {
        var str = config[path] 
                  ?? throw new ConfigurationException($"Missing '{path}' parameter at app settings.");
        if (!int.TryParse(str, out var value))
            throw new ConfigurationException($"Config parameter '{path}' must be integer.");
        return value;
    }
}