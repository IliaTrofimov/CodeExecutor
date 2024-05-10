using CodeExecutor.DB.Abstractions;
using Microsoft.Extensions.Configuration;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace CodeExecutor.DB;

public sealed class DbConfig : IDbConfig
{
    private readonly string connectionString;

    public string ConnectionString
    {
        get => connectionString;
        private init =>
            connectionString = value ??
                               throw new ArgumentNullException(nameof(ConnectionString),
                                   "Missing ConnectionString parameter");
    }

    public DbConfig(IConfiguration config) { ConnectionString = config["PostgreSql"]!; }
}