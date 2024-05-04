using Microsoft.Extensions.Configuration;

namespace CodeExecutor.DB.ServicesConfiguration;

public sealed class DbConfig : IDbConfig
{
    private string connectionString;

    public string ConnectionString
    {
        get => connectionString;
        private init => connectionString = value ?? throw new ArgumentNullException(nameof(ConnectionString), "Missing ConnectionString parameter");
    }

    public DbConfig(IConfiguration config)
    {
        ConnectionString = config["PostgreSql"]!;
    }
}