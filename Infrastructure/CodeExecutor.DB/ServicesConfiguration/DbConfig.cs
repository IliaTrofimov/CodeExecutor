using Microsoft.Extensions.Configuration;

namespace CodeExecutor.DB.ServicesConfiguration;

public sealed class DbConfig
{
    private string connectionString;

    public string ConnectionString
    {
        get => connectionString;
        private set => connectionString = value ?? throw new ArgumentNullException(nameof(ConnectionString), "Missing ConnectionString parameter");
    }

    public DbConfig(IConfiguration config)
    {
        ConnectionString = config["PostgreSql"]!;
    }
}