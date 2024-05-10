using CodeExecutor.Common.Models.Exceptions;
using Microsoft.Extensions.Configuration;


namespace CodeExecutor.Common.Models.Configs;

public static class ConfigurationExtensions
{
    public static string GetValue(this IConfiguration config, string path) =>
        config[path]
        ?? throw new ConfigurationException($"Missing '{path}' parameter at app settings.");

    public static bool TryGetValue(this IConfiguration config, string path, out string? value)
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